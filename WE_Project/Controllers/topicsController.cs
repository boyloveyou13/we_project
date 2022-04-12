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
            if (Session["us"] == null || Convert.ToInt32(Session["state"]) > 2)
            {
                return RedirectToAction("Index", "Login");
            }
            if (msg == 1)
                ViewBag.ErrorMessage = "This topic contains idea, which cannot be deleted";
            else if(msg == 2)
                ViewBag.ErrorMessage = "This topic contains no data";
            var topic = db.topic.ToList();
            return View(topic);
        }
        public ActionResult SelectTopic()
        {
            return PartialView(db.topic.ToList());
        }

        // GET: topics/Details/5
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
            if (Session["us"] == null || Convert.ToInt32(Session["state"]) > 2)
            {
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                topic.topic_name = topic.topic_name.Trim();
                if(topic.describe != null)
                    topic.describe = topic.describe.Trim();
                var topicDB = db.topic.Where(t => t.topic_name == topic.topic_name);
                if (topicDB.ToList().Count == 0)
                {
                    if (topic.closure_date != null)
                    {
                        if ((DateTime.Compare((DateTime)topic.closure_date, DateTime.Now.Date)) < 0)
                        {
                            ViewBag.ErrorMessage = "Closure date cannot be earlier than date now";
                            return View(topic);
                        }
                        if (topic.final_date != null)
                        {
                            if (DateTime.Compare((DateTime)topic.closure_date, (DateTime)topic.final_date) > 0)
                            {
                                ViewBag.ErrorMessage = "Closure date cannot be earlier than final date";
                                return View(topic);
                            }
                        }
                    }else
                    {
                        ViewBag.ErrorMessage = "Closure date cannot be empty";
                        return View(topic);
                    }
                    if (topic.final_date != null)
                    {
                        if ((DateTime.Compare((DateTime)topic.final_date, DateTime.Now.Date)) < 0)
                        {
                            ViewBag.ErrorMessage = "Final date cannot be earlier than date now";
                            return View(topic);
                        }
                    }else
                    {
                        ViewBag.ErrorMessage = "Final date cannot be empty";
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
            if (Session["us"] == null || Convert.ToInt32(Session["state"]) > 2)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            topic topic = db.topic.Find(id);
            if (topic == null)
            {
                return HttpNotFound();
            }
            DateTime close = (DateTime)topic.closure_date;
            DateTime final = (DateTime)topic.final_date;
            ViewBag.closure = close.ToString("MM/dd/yy");
            ViewBag.final = final.ToString("MM/dd/yy");
            return View(topic);
        }

        // POST: topics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "topic_id,topic_name,describe,closure_date,final_date")] topic topic)
        {
            if (Session["us"] == null || Convert.ToInt32(Session["state"]) > 2)
            {
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                topic.topic_name = topic.topic_name.Trim();
                if(topic.describe != null)
                    topic.describe = topic.describe.Trim();
                var topicID = db.topic.AsNoTracking().Where(t=>t.topic_id == topic.topic_id).ToList();
                if(topic.closure_date == null)
                {
                    topic.closure_date = topicID.First().closure_date;
                }
                if(topic.final_date == null)
                {
                    topic.final_date = topicID.First().final_date;
                }
                var topicDB = db.topic.Where(t => t.topic_name == topic.topic_name && t.topic_id != topic.topic_id);
                if (topicDB.ToList().Count == 0)
                {
                    if(topic.closure_date != null)
                    {
                        if ((DateTime.Compare((DateTime)topic.closure_date, DateTime.Now.Date)) < 0)
                        {
                            ViewBag.ErrorMessage = "Closure date cannot be earlier than date now";
                            return View(topic);
                        }
                        if(topic.final_date != null)
                        {
                            if (DateTime.Compare((DateTime)topic.closure_date, (DateTime)topic.final_date) > 0)
                            {
                                ViewBag.ErrorMessage = "Closure date cannot be earlier than final date";
                                return View(topic);
                            }
                        }
                    }
                    if(topic.final_date != null)
                    {
                        if ((DateTime.Compare((DateTime)topic.final_date, DateTime.Now.Date)) < 0)
                        {
                            ViewBag.ErrorMessage = "Final date cannot be earlier than date now";
                            return View(topic);
                        }
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


        public ActionResult Close(int? id)
        {
            topic topic = db.topic.Find(id);

            if (topic.closure_date != null)
            {
                if ((DateTime.Compare((DateTime)topic.closure_date, DateTime.Now.Date)) > 0)
                {
                    topic.closure_date = DateTime.Now.AddDays(-1);                  
                }
            }
            if (topic.final_date != null)
            {
                if ((DateTime.Compare((DateTime)topic.final_date, DateTime.Now.Date)) > 0)
                {
                    topic.final_date = DateTime.Now.AddDays(-1);
                }
            }
            db.Entry(topic).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: topics/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["us"] == null || Convert.ToInt32(Session["state"]) > 2)
            {
                return RedirectToAction("Index", "Login");
            }
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
