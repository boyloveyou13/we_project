using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WE_Project.Models;

namespace WE_Project.Controllers
{
    public class ExportController : Controller
    {
        private squadnerdEntities db = new squadnerdEntities();
        // GET: Export
        public ActionResult Index(int? id)
        {
            if (Session["us"] == null || Convert.ToInt32(Session["state"].ToString()) >2)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["us"] == null || Convert.ToInt32(Session["state"].ToString()) > 2)
            {
                return RedirectToAction("Index", "Login");
            }
            if (id != null)
            {
                var list = db.idea.Where(t => t.topic_id == id).ToList();
                ViewBag.count = list.OrderByDescending(t => t.comment.Count).First().comment.Count;                
                return View(list.ToList());
            }
            else
                return RedirectToAction("Index", "Home");
            
        }



        [HttpPost]
        public FileResult ExportCSV(int? id)
        {
            var list = db.idea.Where(t => t.topic_id == id).ToList();
            var count = list.OrderByDescending(t => t.comment.Count).First().comment.Count;
            StringBuilder file = new StringBuilder();
            file.Append("Date;Account;Title;Content;Views;Thumbs up;Thumbs down");
            for(int i =1;i<=count;i++)
            {
                file.Append(";Comment " + i);
            }
            file.Append("\r\n");
            foreach(var l in list)
            {
                file.Append(l.idea_date.ToString() + ';' + l.account.email + ';' + l.idea_title.Replace(";",",") + ';' + l.idea_content.Replace(";", ",").Replace("\r","").Replace("\n", ". ") + ';' + l.views.ToString() + ';' + l.thumbs_up.ToString()
                    + ';' + l.thumbs_down.ToString());

                foreach(var c in l.comment)
                {
                    file.Append(';' + c.comment_content.Replace(";", ",").Replace("\n",".").Replace("\r","."));
                }
                file.Append("\r\n");
            }
            return File(Encoding.UTF8.GetBytes(file.ToString()), "text/csv",  list.First().topic.topic_name + "Data.csv");
        }
    }
}