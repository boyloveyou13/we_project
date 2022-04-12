using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
                var list = db.idea.Where(t => t.topic_id == id).OrderByDescending(t => t.topic_id).ToList();
                if(list.Count != 0)
                {
                    ViewBag.count = list.OrderByDescending(t => t.comment.Count).First().comment.Count;
                }else
                {
                    return RedirectToAction("Index","topics", new { msg = 2 });
                }
                           
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
            file.Append("Idea ID;Date;Account;Title;Content;Views;Thumbs up;Thumbs down;Note");
            for(int i =1;i<=count;i++)
            {
                file.Append(";Comment " + i);
            }
            file.Append("\r\n");
            foreach(var l in list)
            {
                if(l.file.Count == 0)
                {
                    file.Append(l.idea_id.ToString() + ';' + l.idea_date.ToString() + ';' + l.account.email + ';' + l.idea_title.Replace(";", ",") + ';' + l.idea_content.Replace(";", ",").Replace("\r", "").Replace("\n", ". ") + ';' + l.views.ToString() + ';' + l.thumbs_up.ToString()
                    + ';' + l.thumbs_down.ToString() + ';');
                }    else
                {
                    file.Append(l.idea_id.ToString() + ';' + l.idea_date.ToString() + ';' + l.account.email + ';' + l.idea_title.Replace(";", ",") + ';' + l.idea_content.Replace(";", ",").Replace("\r", "").Replace("\n", ". ") + ';' + l.views.ToString() + ';' + l.thumbs_up.ToString()
                    + ';' + l.thumbs_down.ToString() + ';' + "There are " + l.file.Count.ToString() + " files attached");
                }    
                

                foreach(var c in l.comment)
                {
                    file.Append(';' + c.comment_content.Replace(";", ",").Replace("\n",".").Replace("\r","."));
                }
                file.Append("\r\n");
            }
             string title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(list.First().topic.topic_name);
                    title = title.Replace(" ", "");
            return File(Encoding.UTF8.GetBytes(file.ToString()), "text/csv", title + "Data.csv");
        }

        [HttpPost]
        public FileResult ExportZIP(int? id)
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                var files = db.file.Where(t => t.idea.topic_id == id).ToList();
                foreach(var f in files)
                {
                    zip.AddEntry("ID"+f.idea_id +"_" +f.file_name,f.file_content);
                }

                using (MemoryStream m = new MemoryStream())
                {
                    string title = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(files.First().idea.topic.topic_name);
                    title = title.Replace(" ", "");
                    zip.Save(m);
                    return File(m.ToArray(), "application/zip", title + "Files.zip");
                }
            }
        }
    }
}