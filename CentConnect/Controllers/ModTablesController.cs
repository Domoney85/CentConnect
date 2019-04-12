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

namespace CentConnect.Controllers
{
    public class ModTablesController : Controller
    {
        private CentPayDBEntities db = new CentPayDBEntities();

        // GET: ModTables
        [Authorize]
        public ActionResult Index()
        {
            string userId =User.Identity.GetUserId();
            var campList = from ca in db.CharAccs where ca.AccId == userId
                           select ca.CampID;
            var ViewQuery = from s in db.ModTables
                            where campList.Contains(s.CampID)
                            orderby s.PostedTime descending
                            select s;
            return PartialView(ViewQuery.ToList());
        }

        // GET: ModTables/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModTable modTable = db.ModTables.Find(id);
            if (modTable == null)
            {
                return HttpNotFound();
            }
            return View(modTable);
        }

        // GET: ModTables/Create
        public ActionResult AddMessage()
        {
            Dictionary<int, string> campList = db.Campaigns.ToDictionary(s => s.CampID, s => s.CampName);
            ViewBag.campList = db.Campaigns.ToList();
            return PartialView("AddMessage");
        }

        // POST: ModTables/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult AddMessage([Bind(Include = "ModId,UserId,PostedTime,CampID,Content,Sig,Heading")] ModTable modTable)
        {
            modTable.UserId = User.Identity.GetUserId();
                //User.Identity.GetUserId();
            modTable.PostedTime = System.DateTime.Now;
            if (ModelState.IsValid)
            {
                db.ModTables.Add(modTable);
                db.SaveChanges();
                return RedirectToAction("Index","Home");
            }

            return View(modTable);
        }

        // GET: ModTables/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModTable modTable = db.ModTables.Find(id);
            if (modTable == null)
            {
                return HttpNotFound();
            }
            return View(modTable);
        }

        // POST: ModTables/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ModId,UserId,PostedTime,CampID,Content,Sig,Heading")] ModTable modTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(modTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(modTable);
        }

        // GET: ModTables/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModTable modTable = db.ModTables.Find(id);
            if (modTable == null)
            {
                return HttpNotFound();
            }
            return View(modTable);
        }

        // POST: ModTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ModTable modTable = db.ModTables.Find(id);
            db.ModTables.Remove(modTable);
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
