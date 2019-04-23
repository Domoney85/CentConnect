using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Web.Mvc;
using CentConnect.Models;
using Microsoft.AspNet.Identity;


namespace CentConnect.Controllers
{
    public class TransactionsController : Controller
    {
        private SessionInfo mySession;
        private CentPayDBEntities db = new CentPayDBEntities();

        // GET: Transactions
        [Authorize]
        public ActionResult Index(int? id)
        {
            
            mySession = new SessionInfo();
            mySession.TempCampID = db.CharAccs.Find(id).CampID;
            mySession.TempGMPass = db.CharAccs.Find(id).IsGM;
            mySession.TempCharID = (int)id;
            Session["SessionData"] = mySession;
            List<TransPackage> tempList = new List<TransPackage>();  
            if (db.CharAccs.Find(id).AccId == User.Identity.GetUserId())
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
                    catch (Exception)
                    {
                        Rec = 0;
                    }
                    try
                    {
                        Sent = charTrans.Where(a => a.SendId == id).Sum(a => a.Amount);
                    }
                    catch (Exception)
                    {
                        Sent = 0;
                    }

                }
                mySession.SumAccount = Rec - Sent;

                foreach (Transaction x in charTrans)
                {
                    int tempId;
                    int tempSent = 0;
                    int tempRec = 0;
                    if (x.RecId != id) { tempId = x.RecId; tempSent = x.Amount; } else { tempId = x.SendId; tempRec = x.Amount; }

                    TransPackage temptrans = new TransPackage(tempId, tempRec, tempSent, x.Reason, x.TransTime);
                    tempList.Add(temptrans);

                }

                ViewBag.SumAccount = mySession.SumAccount;
                // "TransId,SendId,RecId,Amount,Reason,Status"
                return View(tempList);
            }
            else
                return Content("Improper Character Identity "+User.Identity.GetUserName());
                
        }

        public ActionResult GMRender()
        {
            mySession = (SessionInfo)Session["SessionData"];
            var UserID = User.Identity.GetUserId();
            if (mySession.TempCampID > 0 && mySession.TempGMPass != null)
            {
                if (db.Campaigns.Find(mySession.TempCampID).GMPass.Trim() == mySession.TempGMPass.Trim())
                {
                    var charList = from x in db.CharAccs
                                   where x.CampID == mySession.TempCampID
                                   select x.CharId;

                    var GMTrans = from c in db.Transactions
                                    where charList.Contains(c.SendId)|| charList.Contains(c.RecId)
                                    orderby c.TransTime descending
                                    select c;
                    return PartialView("_GMRender", GMTrans.ToList());
                }
                return Content("<H2>Welcome " + db.CharAccs.Find(mySession.TempCharID).CharName + "</H2></br> Your GMPassword did not work");
            }
            // "TransId,SendId,RecId,Amount,Reason,Status"
            else
                return Content("<H2>Welcome " + db.CharAccs.Find(mySession.TempCharID).CharName + "</H2>");

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
            mySession = (SessionInfo)Session["SessionData"];
            transaction.Status = true;
            transaction.TransTime = System.DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);
                await db.SaveChangesAsync();
                return RedirectToAction("Index" + "/" + mySession.TempCharID);
            }

            return View(transaction);
        }



        // GET: Transactions/Create
        public ActionResult PlayerSend(int? Id)
        {
            mySession = (SessionInfo)Session["SessionData"];
            ViewBag.ErrorMessage = mySession.errorFundMessage;
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
            mySession = (SessionInfo)Session["SessionData"];
            transaction.SendId = mySession.TempCharID;
            transaction.Status = true;
            transaction.TransTime = System.DateTime.Now;
            transaction.Amount = Int32.Parse(transaction.Amount.ToString());
            if (ModelState.IsValid)
            {
                if (transaction.Amount <= mySession.SumAccount)
                {
                    ViewBag.ErrorMessage = "";
                    db.Transactions.Add(transaction);
                    await db.SaveChangesAsync();
                    mySession.errorFundMessage = "";
                    return RedirectToAction("Index" + "/" + mySession.TempCharID);
                }
                else
                {
                    mySession.errorFundMessage = "Insufficient Funds";
                    return RedirectToAction("Index" + "/" + mySession.TempCharID);
                }
            }
            mySession.errorFundMessage = "";
            return RedirectToAction("Index" + "/" + mySession.TempCharID);
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
                         where x.CampID == mySession.TempCampID
                         select x;
            return View("GMAccount", db.SummaryAccs.ToList());
        }
    }
}
