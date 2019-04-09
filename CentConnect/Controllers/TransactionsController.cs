using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CentConnect.Models;
using Microsoft.AspNet.Identity;


namespace CentConnect.Controllers
{
    public class TransactionsController : Controller
    {
        private CentPayDBEntities db = new CentPayDBEntities();

        // GET: Transactions
        public async Task<ActionResult> Index(int? id)
        {

            List<TransPackage> tempList = new List<TransPackage>();
            var UserID = User.Identity.GetUserId();
            if (db.CharAccs.Find(id).AccId == UserID)
            {
                var charTrans = from c in db.Transactions
                                where c.SendId.ToString() == id.ToString() || c.RecId.ToString() == id.ToString()
                                orderby c.TransTime descending
                                select c;

                int Rec = 0;
                int Sent = 0;
                //ViewBag.ActiveList = activeChar.ToList();
                if (charTrans.Count() > 0)
                {
                    try
                    {
                        Rec = charTrans.Where(a => a.RecId == id).Sum(a => a.Amount);
                    }
                    catch (Exception e)
                    {
                        Rec = 0;
                    }
                    try
                    {
                        Sent = charTrans.Where(a => a.SendId == id).Sum(a => a.Amount);
                    }
                    catch (Exception e)
                    {
                        Sent = 0;
                    }

                }
                SessionInfo.TempCampID = db.CharAccs.Find(id).CampID;
                SessionInfo.TempGMPass = db.CharAccs.Find(id).IsGM;
                SessionInfo.TempCharID = (int)id;
                SessionInfo.SumAccount = Rec - Sent;

                foreach (Transaction x in charTrans)
                {
                    int tempId;
                    int tempSent = 0;
                    int tempRec = 0;
                    if (x.RecId != id) { tempId = x.RecId; tempSent = x.Amount; } else { tempId = x.SendId; tempRec = x.Amount; }

                    TransPackage temptrans = new TransPackage(tempId, tempRec, tempSent, x.Reason, x.TransTime);
                    tempList.Add(temptrans);

                }

                ViewBag.SumAccount = SessionInfo.SumAccount;
                // "TransId,SendId,RecId,Amount,Reason,Status"
                return View(tempList);
            }
            else
                return Content("Improper Character Identity");
        }

        public ActionResult GMRender()
        {

            var UserID = User.Identity.GetUserId();
            if (SessionInfo.TempCampID > 0 && SessionInfo.TempGMPass != null)
            {
                if (db.Campaigns.Find(SessionInfo.TempCampID).GMPass.Trim() == SessionInfo.TempGMPass.Trim())
                {
                    var charList = from x in db.CharAccs
                                   where x.CampID == SessionInfo.TempCampID
                                   select x.CharId;

                    var GMTrans = from c in db.Transactions
                                    where charList.Contains(c.SendId)|| charList.Contains(c.RecId)
                                    orderby c.TransTime descending
                                    select c;
                    return PartialView("_GMRender", GMTrans.ToList());
                }
                return Content("<H2>Welcome " + db.CharAccs.Find(SessionInfo.TempCharID).CharName + "</H2></br> Your GMPassword did not work");
            }
            // "TransId,SendId,RecId,Amount,Reason,Status"
            else
                return Content("<H2>Welcome " + db.CharAccs.Find(SessionInfo.TempCharID).CharName + "</H2>");

        }


        // GET: Transactions/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = await db.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            ViewBag.charList = db.CharAccs.ToList();
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "TransId,SendId,RecId,Amount,Reason,Status")] Transaction transaction)
        {
            transaction.Status = true;
            transaction.TransTime = System.DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);
                await db.SaveChangesAsync();
                return RedirectToAction("Index" + "/" + SessionInfo.TempCharID);
            }

            return View(transaction);
        }



        // GET: Transactions/Create
        public ActionResult PlayerSend(int? Id)
        {
            ViewBag.ErrorMessage = SessionInfo.errorFundMessage;
            var campID = db.CharAccs.Find(Id).CampID;
            var activeChar = from x in db.CharAccs
                             where x.Removed != true && x.CampID == campID
                             select x;
            ViewBag.charList = activeChar.ToList();

            return PartialView("PlayerSend");
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index([Bind(Include = "TransId,SendId,RecId,Amount,Reason,Status")] Transaction transaction)
        {
            transaction.SendId = SessionInfo.TempCharID;
            transaction.Status = true;
            transaction.TransTime = System.DateTime.Now;
            transaction.Amount = Int32.Parse(transaction.Amount.ToString());
            if (ModelState.IsValid)
            {
                if (transaction.Amount <= SessionInfo.SumAccount)
                {
                    ViewBag.ErrorMessage = "";
                    db.Transactions.Add(transaction);
                    await db.SaveChangesAsync();
                    SessionInfo.errorFundMessage = "";
                    return RedirectToAction("Index" + "/" + SessionInfo.TempCharID);
                }
                else
                {
                    SessionInfo.errorFundMessage = "Insufficient Funds";
                    return RedirectToAction("Index" + "/" + SessionInfo.TempCharID);
                }
            }
            SessionInfo.errorFundMessage = "";
            return RedirectToAction("Index" + "/" + SessionInfo.TempCharID);
        }




        // GET: Transactions/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = await db.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "TransId,SendId,RecId,Amount,Reason,Status")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = await db.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Transaction transaction = await db.Transactions.FindAsync(id);
            db.Transactions.Remove(transaction);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult GMAccount()
        {
            var SumAcc = from x in db.SummaryAccs
                         where x.CampID == SessionInfo.TempCampID
                         select x;
            return View("GMAccount", db.SummaryAccs.ToList());
        }
    }
}
