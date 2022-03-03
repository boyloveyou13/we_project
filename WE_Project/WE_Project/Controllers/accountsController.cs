using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            account account = db.account.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: accounts/Create
        public ActionResult Create(int? t )
        {
            ViewBag.t = (int)t;
            ViewBag.department_id = new SelectList(db.department, "department_id", "department_name");
            return View();
        }

        // POST: accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "account_id,email,password,state,fname,gender,phone,position,department_id,img")] account account)
        {
            if (ModelState.IsValid)
            {
                var accountDB = db.account.Where(t => t.email == account.email);
                if(accountDB.ToList().Count ==0)
                {
                    MD5 md5 = new MD5CryptoServiceProvider();
                    string pass = BitConverter.ToString(md5.ComputeHash(ASCIIEncoding.Default.GetBytes(account.password)));
                    account.password = pass;
                    db.account.Add(account);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { t = account.state });
                }else
                {
                    ViewBag.ErrorMessage = "Email is existed";
                    return View(account);
                }
                
            }

            ViewBag.department_id = new SelectList(db.department, "department_id", "department_name", account.department_id);
            return View(account);
        }

        // GET: accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            account account = db.account.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            ViewBag.department_id = new SelectList(db.department, "department_id", "department_name", account.department_id);
            return View(account);
        }

        public ActionResult ChangePassword(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            account account = db.account.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            ViewBag.department_id = new SelectList(db.department, "department_id", "department_name", account.department_id);
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword([Bind(Include = "account_id,email,CurrentPassword,password,ConfirmPassword,state,fname,gender,phone,position,department_id,img")] account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                if (account.CurrentPassword != null && account.CurrentPassword != String.Empty)
                {


                    var accountDB = db.account.Where(t => t.account_id == account.account_id).Where(t => t.password == account.CurrentPassword);
                    if (accountDB.ToList().Count != 0)
                    {
                        db.SaveChanges();
                        return RedirectToAction("Details", new { id = account.account_id });
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Current password is incorrect";
                        ViewBag.department_id = new SelectList(db.department, "department_id", "department_name", account.department_id);
                        return View(account);
                    }
                }
                
            }
            ViewBag.department_id = new SelectList(db.department, "department_id", "department_name", account.department_id);
            return View(account);
        }

        // POST: accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.department_id = new SelectList(db.department, "department_id", "department_name", account.department_id);
            return View(account);
        }

        // GET: accounts/Delete/5
        public ActionResult Delete(int? id)
        {
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
