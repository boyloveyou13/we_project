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
        public ActionResult Index(int? msg)
        {
            if (msg == 1)
                ViewBag.ErrorMessage = "This topic contains idea, which cannot be deleted";
            var topic = db.topic.ToList();
            foreach(var item in topic)
            {
                item.idea_count = db.idea.Where(t => t.topic_id == item.topic_id).Count();
                if(item.closure_date != null && item.final_date != null)
                {
                    item.status = DateTime.Compare((DateTime)item.closure_date, DateTime.Now.Date) >= 0 ? "Opening" :
                  DateTime.Compare((DateTime)item.final_date, DateTime.Now.Date) >= 0 ? "Closing" : "Disable";
                }else
                {
                    item.status = "Opening";
                }
              
            }
            return View(topic);
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
                    //DateTime closure = (DateTime)topic.closure_date;
                    if ((DateTime.Compare((DateTime)topic.closure_date, DateTime.Now.Date)) < 0)
                    {
                        ViewBag.ErrorMessage = "Closure date cannot be earlier than date now";
                        return View(topic);
                    }
                    if (DateTime.Compare((DateTime)topic.closure_date, (DateTime)topic.final_date) > 0)
                    {
                        ViewBag.ErrorMessage = "Closure date cannot be earlier than final date";
                        return View(topic);
                    }
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

                var topicDB = db.topic.Where(t => t.topic_name == topic.topic_name && t.topic_id != topic.topic_id);
                if (topicDB.ToList().Count == 0)
                {
                    if((DateTime.Compare((DateTime)topic.closure_date,  DateTime.Now.Date)) < 0)
                    {
                        ViewBag.ErrorMessage = "Closure date cannot be earlier than date now";
                        return View(topic);
                    }
                    if(DateTime.Compare((DateTime)topic.closure_date,(DateTime)topic.final_date) >0)
                    {
                        ViewBag.ErrorMessage = "Closure date cannot be earlier than final date";
                        return View(topic);
                    }
                    db.Entry(topic).State = System.Data.Entity.EntityState.Modified;
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
                    return RedirectToAction("Index", new { msg = 1 });
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
