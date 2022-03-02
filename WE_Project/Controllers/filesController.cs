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
    public class filesController : Controller
    {
        private squadnerdEntities db = new squadnerdEntities();

        // GET: files
        public ActionResult Index()
        {
            var file = db.file.Include(f => f.idea);
            return View(file.ToList());
        }

        // GET: files/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            file file = db.file.Find(id);
            if (file == null)
            {
                return HttpNotFound();
            }
            return View(file);
        }

        // GET: files/Create
        public ActionResult Create()
        {
            ViewBag.idea_id = new SelectList(db.idea, "idea_id", "idea_content");
            return View();
        }

        // POST: files/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "file_id,idea_id,file_name")] file file)
        {
            if (ModelState.IsValid)
            {
                db.file.Add(file);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idea_id = new SelectList(db.idea, "idea_id", "idea_content", file.idea_id);
            return View(file);
        }

        // GET: files/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            file file = db.file.Find(id);
            if (file == null)
            {
                return HttpNotFound();
            }
            ViewBag.idea_id = new SelectList(db.idea, "idea_id", "idea_content", file.idea_id);
            return View(file);
        }

        // POST: files/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "file_id,idea_id,file_name")] file file)
        {
            if (ModelState.IsValid)
            {
                db.Entry(file).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idea_id = new SelectList(db.idea, "idea_id", "idea_content", file.idea_id);
            return View(file);
        }

        // GET: files/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            file file = db.file.Find(id);
            if (file == null)
            {
                return HttpNotFound();
            }
            return View(file);
        }

        // POST: files/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            file file = db.file.Find(id);
            db.file.Remove(file);
            db.SaveChanges();
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
