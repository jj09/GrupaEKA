using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GrupaEka.Models;

namespace GrupaEka.Controllers
{
    [Authorize(Roles = "Admin")]
    public class NewsController : Controller
    {
        GrupaEkaDB db = new GrupaEkaDB();

        //
        // GET: /News/
        public ActionResult Index()
        {
            var news = db.News.OrderByDescending(n => n.Date);

            return View(news);
        }

        //
        // GET: /News/Add
        public ActionResult Create()
        {
            ViewBag.Now = DateTime.Now;
            NewsTimeViewModel newNewsTime = new NewsTimeViewModel();
            newNewsTime.News = new News();
            newNewsTime.News.Date = DateTime.Now;
            return View(newNewsTime);
        }

        //
        // POST: /News/Add
        [HttpPost]
        public ActionResult Create(NewsTimeViewModel newNewsTime)
        {
            if (ModelState.IsValid)
            {
                News newNews = newNewsTime.News;
                int hour, minutes;
                Int32.TryParse(newNewsTime.Hour,out hour);
                Int32.TryParse(newNewsTime.Minutes,out minutes);
                TimeSpan time = new TimeSpan(hour, minutes, 0);
                newNews.Date = newNews.Date + time;
                db.News.Add(newNews);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(newNewsTime);
            }
        }

        // 
        // GET: /News/Details
        public ActionResult Details(int id)
        {
            News news = db.News.Find(id);
            if (news == null)
                return RedirectToAction("Index");
            return View("Details", news);
        }

        // 
        // GET: /News/Edit
        public ActionResult Edit(int id)
        {
            News news = db.News.Find(id);
            if (news == null)
                return RedirectToAction("Index");
            NewsTimeViewModel editNews = new NewsTimeViewModel();
            editNews.News = news;
            editNews.Hour = news.Date.Hour.ToString();
            editNews.Minutes = news.Date.Minute.ToString();
            return View(editNews);
        }

        // 
        // POST: /News/Edit 
        [HttpPost]
        public ActionResult Edit(NewsTimeViewModel model)
        {
            try
            {
                var news = db.News.Find(model.News.ID);
                int hour, minutes;
                Int32.TryParse(model.Hour, out hour);
                Int32.TryParse(model.Minutes, out minutes);
                TimeSpan time = new TimeSpan(hour, minutes, 0);
                news.Date = model.News.Date + time;
                news.Text = model.News.Text;
                news.Title = model.News.Title;
                UpdateModel(news);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = model.News.ID });
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Edit Failure, see inner exception");
            }

            return View(model);
        }

        // 
        // GET: /News/Delete
        public ActionResult Delete(int id)
        {
            News news = db.News.Find(id);
            if (news == null)
                return RedirectToAction("Index");
            return View(news);
        }

        // 
        // POST: /News/Delete 
        [HttpPost]
        public RedirectToRouteResult Delete(int id, FormCollection collection)
        {
            News news = db.News.Find(id);
            db.News.Remove(news);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
