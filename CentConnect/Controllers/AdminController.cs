using System;
using System.Globalization;
using System.Linq;

using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using CentConnect.Models;
using System.Data;



namespace CentConnect.Controllers
{
    public class AdminController : Controller
    {
        private CentPayDBEntities centdb = new CentPayDBEntities();
        private CentConnectLogEntities centLogdb = new CentConnectLogEntities();

        /// Application DB context
        /// </summary>
        protected ApplicationDbContext ApplicationDbContext { get; set; }

        /// <summary>
        /// User manager - attached to application DB context
        /// </summary>
        protected UserManager<ApplicationUser> UserManager { get; set; }
        // GET: Admin
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {

                return View();
            }
            else
            {
                return View("Error");
            }
        }
        public ActionResult UserRoles()
        {
            
                return View(centLogdb.UserRoles);
            /*
            if (User.IsInRole("Admin"))
            {

                return View(centLogdb.UserRoles);
            }
            else
            {
                return View("Error");
            }*/
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAdmin(string emailName)
        {
            string EmailName = emailName;
             string userid = (from c in centLogdb.AspNetUsers
                         where c.Email.ToString().Trim() == EmailName.Trim()
                         select c.Id).First();
            
            centLogdb.AddUserToRole(userid,"2");
            return RedirectToAction("UserRoles");
        }
        //Add Authorize Role Admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveAdmin(string emailName)
        {

            string EmailName = emailName;
            string userid = (from c in centLogdb.AspNetUsers
                             where c.Email.ToString().Trim() == EmailName.Trim()
                             select c.Id).First();

            centLogdb.RemoveUserRole(userid, "2");
            return RedirectToAction("UserRoles");
        }

    }
}