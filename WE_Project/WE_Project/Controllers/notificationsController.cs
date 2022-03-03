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
    public class notificationsController : Controller
    {
        private squadnerdEntities db = new squadnerdEntities();

        // GET: notifications
        public ActionResult Index()
        {
            var notification = db.notification.Include(n => n.account).Include(n => n.comment);
            return View(notification.ToList());
        }

        // GET: notifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            notification notification = db.notification.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // GET: notifications/Create
        public ActionResult Create()
        {
            ViewBag.account_id = new SelectList(db.account, "account_id", "email");
            ViewBag.comment_id = new SelectList(db.comment, "comment_id", "comment_content");
            return View();
        }

        // POST: notifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "notification_id,comment_id,state,account_id")] notification notification)
        {
            if (ModelState.IsValid)
            {
                db.notification.Add(notification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.account_id = new SelectList(db.account, "account_id", "email", notification.account_id);
            ViewBag.comment_id = new SelectList(db.comment, "comment_id", "comment_content", notification.comment_id);
            return View(notification);
        }

        // GET: notifications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            notification notification = db.notification.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            ViewBag.account_id = new SelectList(db.account, "account_id", "email", notification.account_id);
            ViewBag.comment_id = new SelectList(db.comment, "comment_id", "comment_content", notification.comment_id);
            return View(notification);
        }

        // POST: notifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "notification_id,comment_id,state,account_id")] notification notification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(notification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.account_id = new SelectList(db.account, "account_id", "email", notification.account_id);
            ViewBag.comment_id = new SelectList(db.comment, "comment_id", "comment_content", notification.comment_id);
            return View(notification);
        }

        // GET: notifications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            notification notification = db.notification.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            notification notification = db.notification.Find(id);
            db.notification.Remove(notification);
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
