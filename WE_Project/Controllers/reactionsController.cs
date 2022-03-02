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
    public class reactionsController : Controller
    {
        private squadnerdEntities db = new squadnerdEntities();

        // GET: reactions
        public ActionResult Index()
        {
            var reaction = db.reaction.Include(r => r.account).Include(r => r.idea);
            return View(reaction.ToList());
        }

        // GET: reactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            reaction reaction = db.reaction.Find(id);
            if (reaction == null)
            {
                return HttpNotFound();
            }
            return View(reaction);
        }

        // GET: reactions/Create
        public ActionResult Create()
        {
            ViewBag.account_id = new SelectList(db.account, "account_id", "email");
            ViewBag.idea_id = new SelectList(db.idea, "idea_id", "idea_content");
            return View();
        }

        // POST: reactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "reaction_id,idea_id,thumb,account_id")] reaction reaction)
        {
            if (ModelState.IsValid)
            {
                db.reaction.Add(reaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.account_id = new SelectList(db.account, "account_id", "email", reaction.account_id);
            ViewBag.idea_id = new SelectList(db.idea, "idea_id", "idea_content", reaction.idea_id);
            return View(reaction);
        }

        // GET: reactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            reaction reaction = db.reaction.Find(id);
            if (reaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.account_id = new SelectList(db.account, "account_id", "email", reaction.account_id);
            ViewBag.idea_id = new SelectList(db.idea, "idea_id", "idea_content", reaction.idea_id);
            return View(reaction);
        }

        // POST: reactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "reaction_id,idea_id,thumb,account_id")] reaction reaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.account_id = new SelectList(db.account, "account_id", "email", reaction.account_id);
            ViewBag.idea_id = new SelectList(db.idea, "idea_id", "idea_content", reaction.idea_id);
            return View(reaction);
        }

        // GET: reactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            reaction reaction = db.reaction.Find(id);
            if (reaction == null)
            {
                return HttpNotFound();
            }
            return View(reaction);
        }

        // POST: reactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            reaction reaction = db.reaction.Find(id);
            db.reaction.Remove(reaction);
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
