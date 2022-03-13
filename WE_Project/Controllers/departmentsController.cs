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
    public class departmentsController : Controller
    {
        private squadnerdEntities db = new squadnerdEntities();

        // GET: departments


        public ActionResult Index(int? msg)
        {
            if (Session["us"] == null || Convert.ToInt32(Session["state"]) != 1)
            {
                return RedirectToAction("Index", "Login");
            }
            if (msg == 1)
                ViewBag.ErrorMessage = "This department contains account, which cannot be deleted";
            return View(db.department.ToList());
        }

        // GET: departments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "department_id,department_name")] department department)
        {
            if (Session["us"] == null || Convert.ToInt32(Session["state"]) != 1)
            {
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                department.department_name = department.department_name.Trim();
                var departmentDB = db.department.Where(t => t.department_name == department.department_name);
                if (departmentDB.ToList().Count == 0)
                {
                    db.department.Add(department);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorMessage = "Name is duplicated";
                    return View(department);
                }
               
            }

            return View(department);
        }

        // GET: departments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["us"] == null || Convert.ToInt32(Session["state"]) != 1)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            department department = db.department.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "department_id,department_name")] department department)
        {
            if (Session["us"] == null || Convert.ToInt32(Session["state"]) != 1)
            {
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                department.department_name = department.department_name.Trim();
                var departmentDB = db.department.Where(t => t.department_name == department.department_name && t.department_id != department.department_id);
                if (departmentDB.ToList().Count == 0)
                {
                    db.Entry(department).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorMessage = "Name is duplicated";
                    return View(department);
                }
            }
            return View(department);
        }

        // GET: departments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["us"] == null || Convert.ToInt32(Session["state"]) != 1)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            department department = db.department.Find(id);
            if (department != null)
            {
                var account = db.account.Where(t => t.department_id == department.department_id);
                if (account.ToList().Count == 0)
                {
                    db.department.Remove(department);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {                 
                    return RedirectToAction("Index", new { msg = 1});
                }
            }
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
