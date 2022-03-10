using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WE_Project.Hubs;
using WE_Project.Models;


namespace WE_Project.Controllers
{
    public class commentsController : Controller
    {
        private squadnerdEntities db = new squadnerdEntities();

        // GET: comments
        public ActionResult Index(int? id)
        {

            var comment = db.comment.Include(c => c.account).Include(c => c.idea).Where(c => c.idea_id == id).OrderBy(c=>c.comment_id);
            return PartialView(comment.ToList());
        }

        // GET: comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comment comment = db.comment.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        [HttpPost]
        public JsonResult Create(int ID, string Comment, bool Enonymous)
        {
            comment comment = new comment();
            var account_id = Convert.ToInt32(Session["id"]);
            comment.account_id = account_id;
            comment.idea_id = ID;
            comment.comment_content = Comment;
            comment.comment_date = DateTime.Now;
            comment.comment_status = Enonymous;
            if (ModelState.IsValid)
            {
                notification notify = new notification();
                var idea = db.idea.Find(ID);               
                idea.idea_recent = DateTime.Now;
                if(idea.account_id != account_id)
                {
                    notify.account_id = idea.account_id;
                    notify.idea_id = ID;
                    notify.state = false;
                    db.notification.Add(notify);
                }
                db.Entry(idea).State = System.Data.Entity.EntityState.Modified;
                db.comment.Add(comment);
                db.SaveChanges();
            }
            return Json(true);
        }

        // GET: comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comment comment = db.comment.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.account_id = new SelectList(db.account, "account_id", "email", comment.account_id);
            ViewBag.idea_id = new SelectList(db.idea, "idea_id", "idea_content", comment.idea_id);
            return View(comment);
        }

        // POST: comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "comment_id,comment_content,account_id,idea_id")] comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.account_id = new SelectList(db.account, "account_id", "email", comment.account_id);
            ViewBag.idea_id = new SelectList(db.idea, "idea_id", "idea_content", comment.idea_id);
            return View(comment);
        }

        // GET: comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comment comment = db.comment.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            comment comment = db.comment.Find(id);
            db.comment.Remove(comment);
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
