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


        public ActionResult Index()
        {

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}