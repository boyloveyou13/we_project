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
    public class ideasController : Controller
    {
        private squadnerdEntities db = new squadnerdEntities();

        // GET: ideas
        public ActionResult Index(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }else
            {
                ViewBag.id = id;               
                var idea = db.idea.Include(i => i.account).Include(i => i.category).Include(i => i.topic).Where(i => i.topic_id == id);
                ViewBag.name = idea.First().topic.topic_name;
                return View(idea.ToList());
            }
            
        }


            // GET: ideas/Details/5
            public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            idea idea = db.idea.Find(id);
            if (idea == null)
            {
                return HttpNotFound();
            }
            return View(idea);
        }

        // GET: ideas/Create
        public ActionResult Create(int? id)
        {
            ViewBag.idTopic = id;
            ViewBag.account_id = new SelectList(db.account, "account_id", "email");
            ViewBag.category_id = new SelectList(db.category, "category_id", "category_name");
            ViewBag.topic_id = new SelectList(db.topic, "topic_id", "topic_name");
            return View();
        }

        // POST: ideas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idea_id,topic_id,account_id,category_id,idea_content,thumbs_up,thumbs_down,views,idea_date,idea_title,idea_trigger")] idea idea)
        {
            idea.thumbs_up = 0;
            idea.thumbs_down = 0;
            idea.views = 0;
            idea.idea_date = DateTime.Now.Date;
            if (ModelState.IsValid)
            {
                db.idea.Add(idea);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = idea.idea_id });

            }

            ViewBag.account_id = new SelectList(db.account, "account_id", "email", idea.account_id);
            ViewBag.category_id = new SelectList(db.category, "category_id", "category_name", idea.category_id);
            ViewBag.topic_id = new SelectList(db.topic, "topic_id", "topic_name", idea.topic_id);
            return View(idea);
        }

        // GET: ideas/Edit/5
        public PartialViewResult Edit(int? id)
        {
            if (id == null)
            {
                
            }
            idea idea = db.idea.Find(id);
            if (idea == null)
            {
                
            }
            ViewBag.account_id = new SelectList(db.account, "account_id", "email", idea.account_id);
            ViewBag.category_id = new SelectList(db.category, "category_id", "category_name", idea.category_id);
            ViewBag.topic_id = new SelectList(db.topic, "topic_id", "topic_name", idea.topic_id);
            return PartialView(idea);
        }

        // POST: ideas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idea_id,topic_id,account_id,category_id,idea_content,thumbs_up,thumbs_down,views,idea_date")] idea idea)
        {
            if (ModelState.IsValid)
            {
                db.Entry(idea).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.account_id = new SelectList(db.account, "account_id", "email", idea.account_id);
            ViewBag.category_id = new SelectList(db.category, "category_id", "category_name", idea.category_id);
            ViewBag.topic_id = new SelectList(db.topic, "topic_id", "topic_name", idea.topic_id);
            return View(idea);
        }

        // GET: ideas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            idea idea = db.idea.Find(id);

            if (idea != null)
            {
                ViewBag.id = idea.topic_id;
                db.idea.Remove(idea);
                db.SaveChanges();
                return RedirectToAction("Index", "ideas", new { id = ViewBag.id });
            }
            return View(idea);
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
