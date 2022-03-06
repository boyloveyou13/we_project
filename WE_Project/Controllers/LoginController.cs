using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WE_Project.Models;

namespace WE_Project.Controllers
{
    public class LoginController : Controller
    {
        private squadnerdEntities db = new squadnerdEntities();

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string email, string password)
        {
            if (ModelState.IsValid)
            {
                //var passwordF = GetMD5(password);
                var data = db.account.Where(s => s.email.Equals(email) && s.password.Equals(password)).ToList();
                if (data.Count() > 0)
                {
                    //add session
                    //Session["FullName"] = data.FirstOrDefault().FirstName + " " + data.FirstOrDefault().LastName;
                    //Session["Email"] = data.FirstOrDefault().Email;
                    Session["id"] = data.FirstOrDefault().account_id;
                    Session["us"] = data.FirstOrDefault().email;
                    Session["state"] = data.FirstOrDefault().state;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ErrorMessage = "E-mail or Password is incorrect";
                    return View();
                }
            }
            return View();
        }


    }
}