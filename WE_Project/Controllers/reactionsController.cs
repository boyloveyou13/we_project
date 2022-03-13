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
    public class reactionsController : Controller
    {
        private squadnerdEntities db = new squadnerdEntities();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        [HttpPost]
        public ActionResult AddThumb(int ID, int Status)
        {
            reaction reaction = new reaction();
            reaction.idea_id = ID;
            reaction.thumb = Status;
            reaction.account_id = Convert.ToInt32(Session["id"]);
            db.reaction.Add(reaction);

            idea idea = db.idea.Find(ID);
            if (Status < 0)
                idea.thumbs_down++;
            else
                idea.thumbs_up++;
            db.Entry(idea).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(true);
        }

        [HttpPost]
        public ActionResult EditThumb(int ID, int Status)
        {
            int account_id = Convert.ToInt32(Session["id"]);
            var list = db.reaction.Where(t => t.idea_id == ID && t.account_id == account_id).ToList();
            reaction reaction = list.First();
            reaction.thumb = Status;
            db.Entry(reaction).State = System.Data.Entity.EntityState.Modified;

            idea idea = db.idea.Find(ID);
            if (Status < 0)
            {
                idea.thumbs_down++;
                idea.thumbs_up--;
            }
            else
            {
                idea.thumbs_up++;
                idea.thumbs_down--;
            }
            db.Entry(idea).State = System.Data.Entity.EntityState.Modified;

            db.SaveChanges();
            return Json(true);
        }

        [HttpPost]
        public ActionResult DeleteThumb(int ID)
        {
            int account_id = Convert.ToInt32(Session["id"]);
            var list = db.reaction.Where(t => t.idea_id == ID && t.account_id == account_id).ToList();
              
            idea idea = db.idea.Find(ID);
            if (list.FirstOrDefault().thumb < 0)
                idea.thumbs_down--;
            else
                idea.thumbs_up--;
            db.Entry(idea).State = System.Data.Entity.EntityState.Modified;
            db.reaction.Remove(list.FirstOrDefault());
            db.SaveChanges();
            return Json(true);
        }
    }
}
