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
    public class topicsController : Controller
    {
        private squadnerdEntities db = new squadnerdEntities();

        // GET: topics
        public ActionResult Index()
        {
            return View(db.topic.ToList());
        }

        public ActionResult TopicMenu()
        {
            return PartialView(db.topic.ToList());
        }

        public ActionResult SelectTopic()
        {
            return PartialView(db.topic.ToList());
        }

        // GET: topics/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            topic topic = db.topic.Find(id);
            if (topic == null)
            {
                return HttpNotFound();
            }
            return View(topic);
        }

        // GET: topics/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: topics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "topic_id,topic_name,describe,closure_date,final_date")] topic topic)
        {
            if (ModelState.IsValid)
            {
                var topicDB = db.topic.Where(t => t.topic_name == topic.topic_name);
                if (topicDB.ToList().Count == 0)
                {
                    db.topic.Add(topic);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorMessage = "Name is duplicated";
                    return View(topic);
                }
               
            }

            return View(topic);
        }

        // GET: topics/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            topic topic = db.topic.Find(id);
            if (topic == null)
            {
                return HttpNotFound();
            }
            return View(topic);
        }

        // POST: topics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "topic_id,topic_name,describe,closure_date,final_date")] topic topic)
        {
            if (ModelState.IsValid)
            {

                var topicDB = db.topic.Where(t => t.topic_name == topic.topic_name);
                if (topicDB.ToList().Count == 0)
                {
                    db.Entry(topic).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorMessage = "Name is duplicated";
                    return View(topic);
                }
            }
            return View(topic);
        }

        // GET: topics/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            topic topic = db.topic.Find(id);
            if (topic != null)
            {
                var idea = db.idea.Where(t => t.topic_id == topic.topic_id);
                if (idea.ToList().Count == 0)
                {
                    db.topic.Remove(topic);
                    db.SaveChanges();
                }
                else
                {
                    ViewBag.ErrorMessage = "This topic contains ideas, which cannot be deleted";
                }
                return RedirectToAction("Index");
            }
            return View(topic);
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
