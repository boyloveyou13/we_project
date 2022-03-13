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
    public class filesController : Controller
    {
        private squadnerdEntities db = new squadnerdEntities();

        // GET: files
        public ActionResult Index(int? id)
        {
            if (Session["us"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var file = db.file.Include(f => f.idea).Where(t=>t.idea_id == id);
            return PartialView(file.ToList());
        }

        public FileContentResult show(int? id)
        {
            file file = db.file.Find(id);
            byte[] imageData = file.file_content;
            if (imageData != null)
                return File(imageData, file.file_type, file.file_name);
            else return null;
        }

        public FileResult DownloadFile(int ID)
        {

            file file = db.file.Find(ID);
            return File(file.file_content, file.file_type, file.file_name);
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
