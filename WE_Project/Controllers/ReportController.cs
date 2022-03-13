using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;
using WE_Project.Models;

namespace WE_Project.Controllers
{
    public class ReportController : Controller
    {
        private squadnerdEntities db = new squadnerdEntities();
        // GET: Report
        public ActionResult Index(int? id)
        {
            if (Session["us"] == null || Convert.ToInt32(Session["state"].ToString()) > 3)
            {
                return RedirectToAction("Index", "Login");
            }
            var account_id = Convert.ToInt32(Session["id"].ToString());

            var account = db.account.Find(account_id);

            
            List <DataPoint> topPopulars = new List<DataPoint>();
            List<DataPoint> topViews = new List<DataPoint>();
            List<DataPoint> categoryReport = new List<DataPoint>();
            var category = db.category.ToList();
            var topView = db.idea.OrderByDescending(t => t.views).Take(10);
            var topPopular = db.idea.OrderByDescending(t => t.thumbs_up - t.thumbs_down).Take(10);
            var count = db.idea.ToList().Count;
            
            if (id == 0)
            {
                if (account.state > 2)
                {
                    topView = db.idea.OrderByDescending(t => t.views).Where(t => t.account.department_id == account.department_id).Take(10);
                    topPopular = db.idea.OrderByDescending(t => t.thumbs_up - t.thumbs_down).Where(t => t.account.department_id == account.department_id).Take(10);
                    count = db.idea.Where(t => t.account.department_id == account.department_id).ToList().Count;

                    foreach (var c in category)
                    {
                        var _category = c.idea.Where(t => t.account.department_id == account.department_id && t.category_id == c.category_id).ToList().Count;
                        categoryReport.Add(new DataPoint(c.category_name, (double)_category / (double)count * 100));
                    }
                }else
                {
                    foreach (var c in category)
                    {
                        var _category = c.idea.Where(t => t.category_id == c.category_id).ToList().Count;
                        categoryReport.Add(new DataPoint(c.category_name, (double)_category / (double)count * 100));
                    }
                }    
            }else
            {
                if(account.state != 3)
                {
                    topView = db.idea.OrderByDescending(t => t.views).Where(t => t.account.department_id == id).Take(10);
                    topPopular = db.idea.OrderByDescending(t => t.thumbs_up - t.thumbs_down).Where(t => t.account.department_id == id).Take(10);
                    count = db.idea.Where(t => t.account.department_id == id).ToList().Count;
                    foreach (var c in category)
                    {
                        var _category = c.idea.Where(t => t.account.department_id == id && t.category_id == c.category_id).ToList().Count;
                        categoryReport.Add(new DataPoint(c.category_name, (double)_category / (double)count * 100));
                    }
                }else
                {
                    return RedirectToAction("Index","Home");
                }
              

            }
       

          
            foreach(var v in topView)
            {
                topViews.Add(new DataPoint(v.idea_title, (double)v.views));
            }
            foreach (var p in topPopular)
            {
                topPopulars.Add(new DataPoint(p.idea_title, (double)(p.thumbs_up - p.thumbs_down)));
            }
            
            ViewBag.PopularPoints = JsonConvert.SerializeObject(topPopulars);
            ViewBag.ViewPoints= JsonConvert.SerializeObject(topViews);
            ViewBag.CategoryPoints = JsonConvert.SerializeObject(categoryReport);


            return View(db.department.ToList());
        }

    }
}