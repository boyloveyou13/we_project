using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WE_Project.Models;


namespace WE_Project.Controllers
{
    public class commentsController : Controller
    {
        private squadnerdEntities db = new squadnerdEntities();

        // GET: comments
        public ActionResult Index(int? id)
        {
            if (Session["us"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var comment = db.comment.Include(c => c.account).Include(c => c.idea).Where(c => c.idea_id == id).OrderBy(c=>c.comment_id);
            return PartialView(comment.ToList());
        }

       

        [HttpPost]
        public JsonResult Create(int ID, string Comment, bool Anonymous)
        {

            comment comment = new comment();
            var account_id = Convert.ToInt32(Session["id"]);
            comment.account_id = account_id;
            comment.idea_id = ID;
            comment.comment_content = Comment.Trim();
            comment.comment_date = DateTime.Now;
            comment.comment_status = Anonymous;
            db.comment.Add(comment);
            db.SaveChanges();
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

                    // Email notification here

                    var senderEmail = new MailAddress("blueswitch.squadnerd@gmail.com", "BlueSwitch - Squadnerd");
                    var receiverEmail = new MailAddress(idea.account.email, "Receiver");
                    var password = "Squadnerd123";
                    var sub = "Some one commented your idea, check it out!";
                    var body = db.account.Find(account_id).fname +" commented your idea <br><br><b>Title: " + idea.idea_title + "</b><br><a href='https://squadnerd.azurewebsites.net/" + Url.Action("Details", "ideas", new { id = comment.idea_id, ed = 0 }) + "'>View Now</a><br><br>Do not reply this email!<br><b>BlueSwitch University</b>";
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
                db.Entry(idea).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true);
        }


        // GET: comments/Delete/5
        public ActionResult Delete(int? id, int? i)
        {
            if (Session["us"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comment comment = db.comment.Find(id);
            if (comment != null)
            {
                db.comment.Remove(comment);
                db.SaveChanges();
                return RedirectToAction("Details","ideas", new {id = i});
            }
            return View(comment);
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
