using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CentConnect.Models;
using Microsoft.AspNet.Identity;
using System.Linq;

using System.ComponentModel;

namespace CentConnect.Controllers
{
    public class CharAccsController : Controller
    {
        private CentPayDBEntities db = new CentPayDBEntities();
        private List<Campaign> campaignList = new List<Campaign>();

        // GET: CharAccs
        [Authorize]
        public ActionResult Index()
        { 
            var UserID = User.Identity.GetUserId();
            var model = from c in db.CharAccs
                        where c.AccId.ToString() == UserID && c.Removed != true
                        select c;
   
            return View(model.ToList());
        }

        // GET: CharAccs/Details/5
        public ActionResult Account(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CharAcc charAcc = db.CharAccs.Find(id);
            if (charAcc == null)
            {
                return HttpNotFound();
            }
            return View(charAcc);
        }



        // GET: CharAccs/Create
        public ActionResult Create()
        {
            Dictionary<int, string> campList = db.Campaigns.ToDictionary(s => s.CampID, s => s.CampName);
            ViewBag.campList = db.Campaigns.ToList();
            return View();
        }

        // POST: CharAccs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CharId,CharName,AccId,IsAlive,IsGM,CampID")] CharAcc charAcc)
        {
            charAcc.AccId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                db.CharAccs.Add(charAcc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(charAcc);
        }

        // GET: CharAccs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CharAcc charAcc = db.CharAccs.Find(id);
            if (charAcc == null)
            {
                return HttpNotFound();
            }
            return View(charAcc);
        }

        // POST: CharAccs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CharId,CharName,IsAlive,IsGM,CampID,AccId")] CharAcc charAcc)
        {
            try
            { 
                if (ModelState.IsValid)
                {
                    db.Entry(charAcc).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
           }

            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
               ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator. Error: "+dbEx);
            }
        
            return View(charAcc);
        }

        // GET: CharAccs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CharAcc charAcc = db.CharAccs.Find(id);
            if (charAcc == null)
            {
                return HttpNotFound();
            }
            return View(charAcc);
        }
        public ActionResult RenderName(int? id)
        {
            if (id != null)
            {
                CharAcc charAcc = db.CharAccs.Find(id);
                return Content(charAcc.CharName);
            }
            else
                return Content("Unknown");
        }
        // POST: CharAccs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CharAcc charAcc = db.CharAccs.Find(id);
            charAcc.Removed=true;
            db.SaveChanges();
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
        
    }

}
