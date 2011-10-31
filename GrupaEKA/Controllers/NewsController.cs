using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GrupaEka.Models;
using System.Data;

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
        // GET: /News/Create
        public ActionResult Create()
        {
            ViewBag.Now = DateTime.Now;
            ViewBag.NewsCategories = db.NewsCategories;
            NewsTimeViewModel newNewsTime = new NewsTimeViewModel();
            newNewsTime.News = new News();
            newNewsTime.News.Date = DateTime.Now;
            return View(newNewsTime);
        }

        //
        // POST: /News/Create
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
                var cats = db.NewsCategories.Where(r => newNewsTime.CategoryIDs.Contains(r.ID));                
                foreach (var cat in cats)
                {
                    newNews.NewsCategories.Add(cat);                    
                }
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
            ViewBag.NewsCategories = db.NewsCategories;
            News news = db.News.Find(id);
            if (news == null)
                return RedirectToAction("Index");
            NewsTimeViewModel editNews = new NewsTimeViewModel();
            editNews.News = news;
            editNews.Hour = news.Date.Hour.ToString();
            editNews.Minutes = news.Date.Minute.ToString();
            ViewBag.CurrentCategories = editNews.News.NewsCategories;
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
                var cats = db.NewsCategories.Where(r => model.CategoryIDs.Contains(r.ID));
                news.NewsCategories.Clear();
                foreach (var cat in cats)
                {
                    news.NewsCategories.Add(cat);
                }
                UpdateModel(news);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = model.News.ID });
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Błąd edycji!");
            }
            ViewBag.NewsCategories = db.NewsCategories;
            ViewBag.CurrentCategories = model.News.NewsCategories;
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

        //
        // GET: /News/CategoryIndex
        public ActionResult CategoryIndex()
        {
            var categories = db.NewsCategories.OrderByDescending(n => n.Name);

            return View(categories);
        }

        //
        // GET: /News/CategoryCreate
        public ActionResult CategoryCreate()
        {
            
            return View();
        }

        //
        // POST: /News/CategoryCreate
        [HttpPost]
        public ActionResult CategoryCreate(NewsCategory category)
        {
            if (ModelState.IsValid)
            {
                //if category exsits we do not need to create it
                var cat = db.NewsCategories.Select(c => c.Name);
                if(cat.Contains(category.Name))
                    return RedirectToAction("CategoryIndex");

                db.NewsCategories.Add(category);
                db.SaveChanges();
                return RedirectToAction("CategoryIndex");
            }
            else
            {
                return View(category);
            }
        }

        // 
        // GET: /News/CategoryDetails
        public ActionResult CategoryDetails(int id)
        {
            NewsCategory category = db.NewsCategories.Find(id);
            if (category == null)
                return RedirectToAction("CategoryIndex");
            return View("CategoryDetails", category);
        }

        // 
        // GET: /News/CategoryEdit
        public ActionResult CategoryEdit(int id)
        {
            NewsCategory category = db.NewsCategories.Find(id);
            if (category == null)
                return RedirectToAction("CategoryIndex");
            return View(category);
        }

        // 
        // POST: /News/CategoryEdit 
        [HttpPost]
        public ActionResult CategoryEdit(NewsCategory newscategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(newscategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CategoryIndex");
            }
            return View(newscategory);
        }

        // 
        // GET: /News/CategoryDelete
        public ActionResult CategoryDelete(int id)
        {
            NewsCategory category = db.NewsCategories.Find(id);
            if (category == null)
                return RedirectToAction("CategoryIndex");
            return View(category);
        }

        // 
        // POST: /News/CategoryDelete 
        [HttpPost]
        public RedirectToRouteResult CategoryDelete(int id, FormCollection collection)
        {
            NewsCategory category = db.NewsCategories.Find(id);
            db.NewsCategories.Remove(category);
            db.SaveChanges();

            return RedirectToAction("CategoryIndex");
        }
    }
}
