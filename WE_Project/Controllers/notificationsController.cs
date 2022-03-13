using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WE_Project.Models;

namespace WE_Project.Controllers
{
    public class notificationsController : Controller
    {
        private squadnerdEntities db = new squadnerdEntities();

        // GET: notifications
        public ActionResult Index()
        {
            if (Session["us"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var id = Convert.ToInt32(Session["id"].ToString());
            var notification = db.notification.Include(n => n.account).Include(n => n.idea).Where(t => t.account_id == id && t.state == false);
            ViewBag.count = notification.ToList().Count;
            return PartialView(notification.ToList());
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
