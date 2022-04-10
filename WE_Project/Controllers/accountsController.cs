using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WE_Project.Models;

namespace WE_Project.Controllers
{
    public class accountsController : Controller
    {
        private squadnerdEntities db = new squadnerdEntities();
        
        // GET: accounts
        public ActionResult Index(int? t)
        {
            if (Session["us"] == null || Session["state"].ToString() != "1")
            {
                return RedirectToAction("Index", "Login");
            }
            var account = db.account.Include(a => a.department);
            if(t != null && t != 0)
            {
                ViewBag.t = (int)t;
                account = db.account.Include(a => a.department).Where(g => g.state == t);   
            }
            return View(account.ToList());
        }

        // GET: accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["us"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            account account = db.account.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            if(account.state == 1)
            {
                ViewBag.state = "Admin";
            }
            else if (account.state == 2)
            {
                ViewBag.state = "QA Manager";
            }
            else if (account.state == 3)
            {
                ViewBag.state = "QA Coordinator";
            }
            else
            {
                ViewBag.state = "Staff";
            }
            if(account.gender == true)
            {
                ViewBag.gender = "Male";
            }else
            {
                ViewBag.gender = "Female";
            }
           

                return View(account);
        }

        // GET: accounts/Create
        public ActionResult Create(int? t, int? e )
        {
            if (Session["us"] == null || Session["state"].ToString() != "1")
            {
                return RedirectToAction("Index", "Login");
            }
            if(t !=null)
                ViewBag.t = (int)t;
            if(e != null)
            {
                switch(e)
                {
                    case 1:
                        ViewBag.ErrorMessage = "Password does not match";
                        break;
                    case 2:
                        ViewBag.ErrorMessage = "Email is existed";
                        break;

                }
            }
            ViewBag.department_id = new SelectList(db.department, "department_id", "department_name");
            return View();
        }

        // POST: accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        public JsonResult Create(string Email, string Password, string Confirm, string State)
        {
            if (Session["us"] == null || Session["state"].ToString() != "1")
            {
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", "Login");
                return Json(new { Url = redirectUrl });
            }
            account account = new account();
            account.email = Email;
            account.password = Password;
            account.state = Convert.ToInt32(State);
            account.gender = false;
            account.isActive = true;
            if (ModelState.IsValid)
            {
                var accountDB = db.account.Where(t => t.email == account.email);
                if (accountDB.ToList().Count == 0)
                {
                    if(Password == Confirm)
                    {
                        MD5 md5 = new MD5CryptoServiceProvider();
                        string pass = BitConverter.ToString(md5.ComputeHash(ASCIIEncoding.Default.GetBytes(account.password)));
                        account.password = pass;
                        db.account.Add(account);
                        db.SaveChanges();
                        var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index","accounts", new { t = account.state });
                        return Json(new { Url = redirectUrl });
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Password does not match";
                        ViewBag.department_id = new SelectList(db.department, "department_id", "department_name", account.department_id);
                        var redirectUrl = new UrlHelper(Request.RequestContext).Action("Create", "accounts", new { t = account.state, e = 1 });
                        return Json(new { Url = redirectUrl });
                    }
                    
                }
                else
                {
                    ViewBag.ErrorMessage = "Email is existed";
                    ViewBag.department_id = new SelectList(db.department, "department_id", "department_name", account.department_id);
                    var redirectUrl = new UrlHelper(Request.RequestContext).Action("Create", "accounts", new { t = account.state, e = 2 });
                    return Json(new { Url = redirectUrl });
                }

            }else
            {
                ViewBag.department_id = new SelectList(db.department, "department_id", "department_name", account.department_id);
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("Create", "accounts", new { t = account.state });
                return Json(new { Url = redirectUrl });
            }


        }

        // GET: accounts/Edit/5
        public ActionResult Edit(int? id, int? s)
        {
            if (Session["us"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            account account = db.account.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            if(s != null)
            {
                Session["s"] = "profile";
            }  else
            {
                Session["s"] = "";
            }    
            ViewBag.department_id = new SelectList(db.department, "department_id", "department_name", account.department_id);
            return View(account);
        }



        public ActionResult Deactivate(int? id)
        {
            if (Session["us"] == null || Session["state"].ToString() != "1")
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            account account = db.account.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            account.isActive = false;
            db.Entry(account).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", new { t = account.state });
        }

        

        public ActionResult ChangePassword(int? id, int? e)
        {
            if (Session["us"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            account account = db.account.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            if (e != null)
            {
                switch (e)
                {
                    case 1:
                        ViewBag.ErrorMessage = "Password is not match";
                        break;
                    case 2:
                        ViewBag.ErrorMessage = "Current password is incorrect";
                        break;
                    case 3:
                        ViewBag.ErrorMessage = "Current password is required";
                        break;
                    default: break;
                }
            }
            ViewBag.id = id;
            return View();
        }

        [HttpPost]
        public JsonResult ChangePassword(int ID, string CurrentPassword, string Password, string Confirm)
        {
            if (Session["us"] == null)
            {
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index","Login");
                return Json(new { Url = redirectUrl });
            }
            account account = db.account.Find(ID);
            if (ModelState.IsValid)
            {
                if (CurrentPassword != null && CurrentPassword != String.Empty)
                {
                    MD5 md5 = new MD5CryptoServiceProvider();
                    string pass = BitConverter.ToString(md5.ComputeHash(ASCIIEncoding.Default.GetBytes(Password)));
                    string Curpass = BitConverter.ToString(md5.ComputeHash(ASCIIEncoding.Default.GetBytes(CurrentPassword)));
                    if(account.password == Curpass)
                    {

                        if (Password == Confirm)
                        {
                            account.password = pass;
                            db.Entry(account).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Details", new { id = ID });
                            return Json(new { Url = redirectUrl });
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Password is not match";
                            ViewBag.department_id = new SelectList(db.department, "department_id", "department_name", account.department_id);
                            var redirectUrl = new UrlHelper(Request.RequestContext).Action("ChangePassword", new { id = ID, e=1 });
                            return Json(new { Url = redirectUrl });
                        }
                    }else
                    {
                        ViewBag.ErrorMessage = "Current password is incorrect";
                        var redirectUrl = new UrlHelper(Request.RequestContext).Action("ChangePassword", new { id = ID, e=2});
                        return Json(new { Url = redirectUrl });
                    }

                }else
                {
                    ViewBag.ErrorMessage = "Current password is required";
                    var redirectUrl = new UrlHelper(Request.RequestContext).Action("ChangePassword", new { id = ID, e=3 });
                    return Json(new { Url = redirectUrl });
                }
                                      
            }else
            {
                ViewBag.department_id = new SelectList(db.department, "department_id", "department_name", account.department_id);
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("ChangePassword", new { id = ID });
                return Json(new { Url = redirectUrl });
            }

        }

        // POST: accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( account account, HttpPostedFileBase avatar)
        {
            if (Session["us"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (avatar != null)
            {
                var supportedTypes = new[] { "png", "jpg", "jpeg" };
                var fileExt = System.IO.Path.GetExtension(avatar.FileName).Substring(1);
                if(!supportedTypes.Contains(fileExt))
                {
                    ViewBag.ErrorMessage = "File Extension Is InValid - Only Upload PNG/JPG/JPEG File";
                    ViewBag.department_id = new SelectList(db.department, "department_id", "department_name", account.department_id);
                    return View(account);
                }
                else if(avatar.ContentLength > 400*1024)
                {
                    ViewBag.ErrorMessage = "File is too large - File should be up to 400KB";
                    ViewBag.department_id = new SelectList(db.department, "department_id", "department_name", account.department_id);
                    return View(account);
                }else
                {
                    byte[] bytes;
                    using(BinaryReader br = new BinaryReader(avatar.InputStream))
                    {
                        bytes = br.ReadBytes(avatar.ContentLength);
                    }
                    account.img = bytes;
                }
                if (ModelState.IsValid)
                {
                    db.Entry(account).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", new { t = account.state });
                }
            }else
            {
                account account2 = db.account.Find(account.account_id);
                account2.fname = account.fname;
                account2.gender = account.gender;
                account2.department_id = account.department_id;
                account2.phone = account.phone;
                account2.position = account.position;
                account2.isActive = account.isActive;
                if (ModelState.IsValid)
                {
                    db.Entry(account2).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    if(Session["s"].ToString() == "profile")
                        return RedirectToAction("Details", new { id = account.account_id });
                    else  
                        return RedirectToAction("Index", new { t = account.state });
                }
            } 

            ViewBag.department_id = new SelectList(db.department, "department_id", "department_name", account.department_id);
            return View(account);
        }

        public FileContentResult show(int? id)
        {
            account account = db.account.Find(id);
            byte[] imageData = account.img;
            if (imageData != null)
                return File(imageData, "image/jpg");
            else return null;
        }

        // GET: accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["us"] == null || Session["state"].ToString() != "1")
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                account account = db.account.Find(id);
                if (account != null)
                {
                    db.account.Remove(account);
                    db.SaveChanges();
                }
                return RedirectToAction("Index", new { t = account.state});
            }
            catch(Exception)
            {
                return RedirectToAction("Index");
            }
            
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
