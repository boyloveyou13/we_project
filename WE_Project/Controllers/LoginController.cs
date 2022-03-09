using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WE_Project.Models;

namespace WE_Project.Controllers
{
    public class LoginController : Controller
    {
        private squadnerdEntities db = new squadnerdEntities();

        // GET: Login


        public ActionResult Index(int? id)
        {
            if (id != null)
                ViewBag.id = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string email, string password, int? id)
        {
            if (ModelState.IsValid)
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                string pass = BitConverter.ToString(md5.ComputeHash(ASCIIEncoding.Default.GetBytes(password)));
                var validData = db.account.Where(s => s.email.Equals(email) && s.isActive == false).ToList();
                if(validData.Count() >0)
                {
                    ViewBag.ErrorMessage = "Your account is deactivate, please contact the admin!";
                    return View();
                }
                else
                {
                    var data = db.account.Where(s => s.email.Equals(email) && s.password.Equals(pass)).ToList();
                    if (data.Count() > 0)
                    {
                        //add session
                        //Session["FullName"] = data.FirstOrDefault().FirstName + " " + data.FirstOrDefault().LastName;
                        //Session["Email"] = data.FirstOrDefault().Email;
                        Session["id"] = data.FirstOrDefault().account_id;
                        Session["us"] = data.FirstOrDefault().email;
                        Session["state"] = data.FirstOrDefault().state;
                        if (data.FirstOrDefault().img == null)
                        {
                            Session["isImage"] = false;
                        }
                        else
                        {
                            Session["isImage"] = true;
                        }

                        if (id == null)
                            return RedirectToAction("Index", "Home");
                        else
                            return RedirectToAction("Details", "Ideas", new { id = id });
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "E-mail or Password is incorrect";
                        return View();
                    }
                }
                
            }
            return View();
        }


    }
}