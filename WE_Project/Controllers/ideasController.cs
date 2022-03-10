using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
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
        public ActionResult Index(int? id, int? sort)
        {
            if (Session["us"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }else
            {
                var idea = db.idea.Include(i => i.account).Include(i => i.category).Include(i => i.topic).Where(i => i.topic_id == id).OrderByDescending(i=>i.idea_id);
                ViewBag.id = id;               
                switch(sort)
                {
                    case 1:
                        ViewBag.sort = "Latest";
                        break;
                    case 2:
                        ViewBag.sort = "Most Popular";
                        idea = idea.OrderByDescending(t => t.thumbs_up - t.thumbs_down);
                        break;
                    case 3:
                        ViewBag.sort = "Most Viewed";
                        idea = idea.OrderByDescending(i => i.views);
                        break;
                    case 4:
                        ViewBag.sort = "Latest Comments";
                        idea = idea.OrderByDescending(i => i.idea_recent);
                        break;
                }

                ViewBag.category_id = new SelectList(db.category, "category_id", "category_name");
                return View(idea.ToList());
            }
            
        }


        // GET: ideas/Details/5
        public ActionResult Details(int? id, int? idNotification)
        {
            if(id != null && Session["us"] == null)
            {
                return RedirectToAction("Index", "Login", new { id = id });
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(idNotification != null)
            {
                notification notification = db.notification.Find(idNotification);
                notification.state = true;
                db.Entry(notification).State = System.Data.Entity.EntityState.Modified;
                
            }
            idea idea = db.idea.Find(id);

            if (Request.Cookies["ViewedPage"] != null)
            {
                if(Request.Cookies["ViewedPage"][string.Format("ideaId_{0}",id)] == null)
                {
                    HttpCookie cookie = (HttpCookie)Request.Cookies["ViewedPage"];
                    cookie[string.Format("ideaId_{0}", id)] = "1";
                    cookie.Expires = DateTime.Now.AddHours(6);
                    Response.Cookies.Add(cookie);
                    idea.views = idea.views + 1;
                    db.Entry(idea).State = System.Data.Entity.EntityState.Modified;

                }
            }else
            {
                HttpCookie cookie = new HttpCookie("ViewedPage");
                cookie[string.Format("ideaId_{0}", id)] = "1";
                cookie.Expires = DateTime.Now.AddHours(6);
                Response.Cookies.Add(cookie);
                idea.views = idea.views + 1;
                db.Entry(idea).State = System.Data.Entity.EntityState.Modified;
            }
            db.SaveChanges();
            
            int _account_id = Convert.ToInt32(Session["id"]);
            var list = db.reaction.Where(t => t.account_id == _account_id && t.idea_id == id).ToList();
            if(list.Count >0)
            {
                reaction reaction = list.First();
                ViewBag.thumb = reaction.thumb;
            }
  
            if (idea == null)
            {
                return HttpNotFound();
            }
            return View(idea);
        }

        // GET: ideas/Create
        public ActionResult Create(int? id)
        {
            var topic = db.topic.Where(t => t.topic_id == id).ToList();
            if(topic.Count >0)
            {
                if (topic.FirstOrDefault().closure_date == null || DateTime.Compare(DateTime.Now.Date, (DateTime)topic.FirstOrDefault().closure_date) <= 0)
                {
                    ViewBag.idTopic = id;
                    ViewBag.account_id = new SelectList(db.account, "account_id", "email");
                    ViewBag.category_id = new SelectList(db.category, "category_id", "category_name");
                    ViewBag.topic_id = new SelectList(db.topic, "topic_id", "topic_name");
                    return PartialView();
                }
            }
                return RedirectToAction("Index",new { id = id });

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

        [HttpPost]
        public JsonResult postIdea()
        {
            idea idea = new idea();
            int _account_id = Convert.ToInt32(Session["id"].ToString());


            var account = db.account.Find(_account_id);
            idea.topic_id = Convert.ToInt32(Request["Topic_id"]);
            idea.category_id =Convert.ToInt32(Request["Category"]);
            idea.account_id = _account_id;
            idea.idea_title = Request["Title"];
            idea.idea_content = Request["Content"];
            idea.thumbs_up = 0;
            idea.thumbs_down = 0;
            idea.views = 0;
            idea.idea_date = DateTime.Now;
            string enon = Request["Enonymous"];
            if(enon == "true")
            {
                idea.idea_trigger = true;
            }else
            {
                idea.idea_trigger = false;
            }        
            db.idea.Add(idea);
            var redirectUrl = "";
            HttpFileCollectionBase files = Request.Files;
            byte[] bytes;
            for(int i = 0;i<files.Count;i++)
            {
                var supportedTypes = new[] { "png", "jpg", "jpeg", "txt", "doc", "docx", "pdf", "xls", "xlsx", "ppt" };
                var fileExt = System.IO.Path.GetExtension(files[i].FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt))
                {
                    redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", new { id = idea.topic_id });
                    return Json(new { Url = redirectUrl });
                }
                else if (files[i].ContentLength > 3*1024 * 1024)
                {
                    redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", new { id = idea.topic_id });
                    return Json(new { Url = redirectUrl });
                }
                else
                {
                    HttpPostedFileBase file = files[i];
                    using (BinaryReader br = new BinaryReader(file.InputStream))
                    {
                        bytes = br.ReadBytes(file.ContentLength);
                    }
                    db.file.Add(new file
                    {
                        idea_id = idea.idea_id,
                        file_content = bytes,
                        file_type = file.ContentType,
                        file_name = Path.GetFileName(file.FileName)
                    });
                }
            }

            notification notify = new notification();
            var list = db.account.Where(t => t.state == 3 && t.department_id == account.department_id).ToList();
            notify.account_id = list.First().account_id;
            notify.idea_id = idea.idea_id;
            notify.state = false;

            db.notification.Add(notify);
            db.SaveChanges();
            redirectUrl = new UrlHelper(Request.RequestContext).Action("Details",new { id = idea.idea_id });
            return Json(new {Url = redirectUrl});
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
            var list = db.reaction.Where(t => t.idea_id == id);
            var list2 = db.file.Where(t => t.idea_id == id);
            if (idea != null)
            {
                foreach(var l in list)
                {
                    db.reaction.Remove(l);
                }
                foreach (var l in list2)
                {
                    db.file.Remove(l);
                }
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
