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
    public class categoriesController : Controller
    {
        private squadnerdEntities db = new squadnerdEntities();

        // GET: categories
        public ActionResult Index(int? msg)
        {
            if (Session["us"] == null || Convert.ToInt32(Session["state"]) > 2)
            {
                return RedirectToAction("Index", "Login");
            }
            if (msg ==1)
                ViewBag.ErrorMessage = "This category contains ideas, which cannot be deleted";
            return View(db.category.ToList());
        }


        // GET: categories/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["us"] == null || Convert.ToInt32(Session["state"]) > 2)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            category category = db.category.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "category_id,category_name,describe")] category category)
        {
            if (Session["us"] == null || Convert.ToInt32(Session["state"]) > 2)
            {
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                var categoryDB = db.category.Where(t => t.category_name == category.category_name);
                if (categoryDB.ToList().Count == 0)
                {
                    if(category.describe != null)
                        category.describe = category.describe.Trim();
                    category.category_name = category.category_name.Trim();
                    db.category.Add(category);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorMessage = "Name is duplicated";
                    return View(category);
                }
            }

            return View(category);
        }

        // GET: categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["us"] == null || Convert.ToInt32(Session["state"]) > 2)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            category category = db.category.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "category_id,category_name,describe")] category category)
        {
            if (Session["us"] == null || Convert.ToInt32(Session["state"]) > 2)
            {
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                var categoryDB = db.category.Where(t => t.category_name == category.category_name && t.category_id != category.category_id);
                if (categoryDB.ToList().Count <= 0)
                {
                    if (category.describe != null)
                        category.describe = category.describe.Trim();
                    category.category_name = category.category_name.Trim();
                    db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorMessage = "Name is duplicated";
                    return View(category);
                }           
            }
            return View(category);
        }

        // GET: categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["us"] == null || Convert.ToInt32(Session["state"]) > 2)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                category category = db.category.Find(id);
                if (category != null)
                {
                    var idea = db.idea.Where(t => t.category_id == category.category_id);
                    if (idea.ToList().Count == 0)
                    {
                        db.category.Remove(category);
                        db.SaveChanges();
                    }
                    else
                    {
                        return RedirectToAction("Index",new { msg = 1 });
                    }
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            catch (Exception)
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
