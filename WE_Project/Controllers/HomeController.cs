using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WE_Project.Models;

namespace WE_Project.Controllers
{
    public class HomeController : Controller
    {
        private squadnerdEntities db = new squadnerdEntities();

        public JsonResult CheckUsernameAvailability(string userdata)
        {
            System.Threading.Thread.Sleep(500);
            var SearchData = db.account.Where(s => s.email == userdata).SingleOrDefault();
            if (SearchData != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        public JsonResult CheckPasswordAvailability(string pass, string confirm)
        {
            if (pass != confirm)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }



        public string CalCulateTime(DateTime date)
        {
            string message = "";
            DateTime currentDate = DateTime.Now;
            TimeSpan timegap = currentDate - date;
            message = string.Concat(date.ToString("MMMM dd, yyyy"), " at ", date.ToString("hh:mm tt"));
            if (timegap.Days > 365)
            {
                message = string.Concat((((timegap.Days) / 30) / 12), " years/s ago");
            }
            else if (timegap.Days > 31)
            {
                message = string.Concat(((timegap.Days) / 30), " month/s ago");
            }
            else if (timegap.Days > 1)
            {
                message = string.Concat(timegap.Days, " day/s ago");
            }
            else if (timegap.Days == 1)
            {
                message = "Posted yesterday";
            }
            else if (timegap.Hours >= 2)
            {
                message = string.Concat(timegap.Hours, " hour/s ago");
            }
            else if (timegap.Hours >= 1)
            {
                message = "an hour ago";
            }
            else if (timegap.Minutes >= 60)
            {
                message = "more than an hour ago";
            }
            else if (timegap.Minutes >= 5)
            {
                message = string.Concat(timegap.Minutes, " minute/s ago");
            }
            else if (timegap.Minutes >= 1)
            {
                message = "a few minute/s ago";
            }
            else
            {
                message = "less than a minute ago";
            }

            return message;
        }

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }


        public ActionResult TermAndConditions()
        {            
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}