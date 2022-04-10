using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
                var topic = db.topic.Find(id);
                DateTime close = (DateTime)topic.closure_date;
                DateTime final = (DateTime)topic.final_date;
                ViewBag.topic_name = topic.topic_name;
                ViewBag.closure = close.ToString("MM/dd/yy");
                ViewBag.final = final.ToString("MM/dd/yy");
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
                var account_id = Convert.ToInt32(Session["id"]);
                var notifyList = db.notification.Where(t => t.state == false && t.idea_id == id && t.account_id == account_id);
                foreach(var l in notifyList)
                {
                    l.state = true;
                    db.Entry(l).State = System.Data.Entity.EntityState.Modified;
                }    
                
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

       

        [HttpPost]
        public JsonResult postIdea()
        {
            idea idea = new idea();
            int _account_id = Convert.ToInt32(Session["id"].ToString());


            var account = db.account.Find(_account_id);
            idea.topic_id = Convert.ToInt32(Request["Topic_id"]);
            idea.category_id =Convert.ToInt32(Request["Category"]);
            idea.account_id = _account_id;
            idea.idea_title = Request["Title"].Trim();
            idea.idea_content = Request["Content"].Trim();
            idea.thumbs_up = 0;
            idea.thumbs_down = 0;
            idea.views = 0;
            idea.idea_date = DateTime.Now;
            idea.idea_recent = DateTime.Now;
            string anon = Request["Anonymous"];
            if(anon == "true")
            {
                idea.idea_trigger = true;
            }else
            {
                idea.idea_trigger = false;
            }        
            db.idea.Add(idea);
            db.SaveChanges();
            int ideaid = idea.idea_id;
            var redirectUrl = "";
            HttpFileCollectionBase files = Request.Files;
            byte[] bytes;
            for(int i = 0;i<files.Count;i++)
            {
                var supportedTypes = new[] { "png", "jpg", "jpeg", "txt", "doc", "docx", "pdf", "xls", "xlsx", "ppt", "csv", "rar", "zip" };
                var fileExt = System.IO.Path.GetExtension(files[i].FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt))
                {
                    redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", new { id = idea.topic_id });
                    return Json(new { Url = redirectUrl });
                }
                else if (files[i].ContentLength > 5*1024 * 1024)
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
            var list = db.account.Where(t => t.state == 3 && t.department_id == account.department_id && t.account_id != _account_id).ToList();
            if(list.Count != 0)
            {
                foreach(var l in list)
                {
                    notify.account_id = l.account_id;
                    notify.idea_id = idea.idea_id;
                    notify.state = false;

                    db.notification.Add(notify);

                    // Email notification here

                    var senderEmail = new MailAddress("blueswitch.squadnerd@gmail.com", "BlueSwitch - Squadnerd");
                    var receiverEmail = new MailAddress(l.email, "Receiver");
                    var password = "Squadnerd123";
                    var sub = "Your staff posted an idea, check it out!";
                    var body = account.fname + " posted an idea <br><br><b>Title: " + idea.idea_title + "</b><br><a href='https://squadnerd.azurewebsites.net/" + Url.Action("Details","ideas",new {id = ideaid, ed=0}) + "'>View Now</a><br><br>Do not reply this email!<br><b>BlueSwitch University</b>";
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,                       
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };
                    MailMessage mess = new MailMessage(senderEmail, receiverEmail);
                    mess.Subject = sub;
                    mess.Body = body;
                    mess.IsBodyHtml = true;

                    smtp.Send(mess);

                }
                

            }

            



            db.SaveChanges();
            redirectUrl = new UrlHelper(Request.RequestContext).Action("Details",new { id = idea.idea_id });
            return Json(new {Url = redirectUrl});
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
            var list3 = db.notification.Where(t => t.idea_id == id);
            var list4 = db.comment.Where(t => t.idea_id == id);
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
                foreach (var l in list3)
                {
                    db.notification.Remove(l);
                }
                foreach (var l in list4)
                {
                    db.comment.Remove(l);
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
