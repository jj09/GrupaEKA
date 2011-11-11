using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GrupaEka.Models;
using System.Data;
using System.Web.Security;
using System.Net.Mail;

namespace GrupaEka.Controllers
{
    public class NewsController : Controller
    {
        private GrupaEkaDB db = new GrupaEkaDB();

        #region News

        //
        // GET: /News/
        [Authorize(Roles="admin")]
        public ActionResult Index()
        {
            var news = db.News.OrderByDescending(n => n.Date);

            return View(news);
        }

        //
        // GET: /News/Create
        [Authorize]
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
        [Authorize]
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
                try
                {
                    var cats = db.NewsCategories.Where(r => newNewsTime.CategoryIDs.Contains(r.ID)).ToList();
                    foreach (var cat in cats)
                    {
                        newNews.NewsCategories.Add(cat);
                    }
                }
                catch (NotSupportedException) { /* 0 categories */ }
                db.News.Add(newNews);
                db.SaveChanges();

                //notify users
                List<Profile> profiles = new List<Profile>();
                foreach(var category in newNews.NewsCategories)
                {
                    var prof = db.Profiles.ToList();
                    foreach(var one in prof)
                    {
                        if(one.Subscriptions.Contains(category))
                            profiles.Add(one);
                    }
                }
                profiles = profiles.Distinct().ToList();
                try
                {
                    MailMessage msg = new MailMessage();
                    msg.From = new MailAddress("noreply@grupaeka.studentlive.pl", "Grupa .NET EKA");
                    foreach (var profile in profiles)
                    {
                        msg.To.Add(Membership.GetUser(profile.UserName).Email);
                    }
                    msg.Subject = "Nowa wiadomość na stronie Grupa .NET EKA";
                    msg.Body = "Na stronie pojawiała się nowa wiadomość. Aby ją zobaczyć kliknij link poniżej:<br />" +
                        "http://" + Request.Url.Host + "/News/Details/" + newNews.ID;
                    msg.IsBodyHtml = true;

                    SmtpClient smtp = new SmtpClient("smtp.live.com", 25);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential("jj09@studentlive.pl", "master");
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(msg);

                    msg.Dispose();
                }
                catch (Exception)
                {
                    ViewBag.NewsCreateError = "Błąd! Nie wysłano powiadomień!";
                }
                //

                return RedirectToAction("Index","Home");
            }
            else
            {
                ViewBag.Now = DateTime.Now;
                ViewBag.NewsCategories = db.NewsCategories;
                return View(newNewsTime);
            }
        }

        // 
        // GET: /News/Details
        public ActionResult Details(int id)
        {
            News news = db.News.Find(id);
            if (news == null)
                return RedirectToAction("Index", "Home");
            return View("Details", news);
        }

        // 
        // GET: /News/Edit
        [Authorize]
        public ActionResult Edit(int id)
        {
            ViewBag.NewsCategories = db.NewsCategories;
            News news = db.News.Find(id);
            if (news == null)
                return RedirectToAction("Index", "Home");
            if (!news.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName, "admin"))
                return RedirectToAction("Details", new { ID = news.ID });
            NewsTimeViewModel editNews = new NewsTimeViewModel();
            editNews.News = news;
            editNews.Hour = news.Date.Hour.ToString();
            editNews.Minutes = news.Date.Minute.ToString();
            ViewBag.CurrentCategories = editNews.News.NewsCategories;
            return View(editNews);
        }

        // 
        // POST: /News/Edit 
        [Authorize]
        [HttpPost]
        public ActionResult Edit(NewsTimeViewModel model)
        {
            try
            {
                var news = db.News.Find(model.News.ID);
                if (news == null)
                    return RedirectToAction("Index", "Home");
                if (!news.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName, "admin"))
                    return RedirectToAction("Details", new { ID = news.ID });
                int hour, minutes;
                Int32.TryParse(model.Hour, out hour);
                Int32.TryParse(model.Minutes, out minutes);
                TimeSpan time = new TimeSpan(hour, minutes, 0);
                news.Date = model.News.Date + time;
                news.Text = model.News.Text;
                news.Title = model.News.Title;
                try
                {
                    var cats = db.NewsCategories.Where(r => model.CategoryIDs.Contains(r.ID));
                    news.NewsCategories.Clear();
                    foreach (var cat in cats)
                    {
                        news.NewsCategories.Add(cat);
                    }
                }
                catch (NotSupportedException) { /* 0 categories */ }
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
        [Authorize]
        public ActionResult Delete(int id)
        {
            News news = db.News.Find(id);
            if (news == null)
                return RedirectToAction("Index", "Home");
            if (!news.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName, "admin"))
                return RedirectToAction("Details", new { ID = news.ID });
            return View(news);
        }

        // 
        // POST: /News/Delete 
        [Authorize]
        [HttpPost]
        public RedirectToRouteResult Delete(int id, FormCollection collection)
        {
            News news = db.News.Find(id);

            if (news == null)
                return RedirectToAction("Index", "Home");
            if (!news.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName, "admin"))
                return RedirectToAction("Details", new { ID = news.ID });

            //delete comments for news - unnecessary when foreign key's Delete Rule=Cascade
            //List<NewsComment> nc = db.NewsComments.Where(c => c.News.ID == id).ToList();
            //foreach(var c in nc)
            //{
            //    db.NewsComments.Remove(c);
            //}
            //end of delete comments

            db.News.Remove(news);
            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        #endregion News

        #region NewsCategory

        //
        // GET: /News/CategoryIndex
        public ActionResult CategoryIndex()
        {
            var categories = db.NewsCategories.OrderByDescending(n => n.Name);

            return View(categories);
        }

        //
        // GET: /News/CategoryCreate
        [Authorize]
        public ActionResult CategoryCreate()
        {            
            return View();
        }

        //
        // POST: /News/CategoryCreate
        [Authorize]
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
        [Authorize]
        public ActionResult CategoryDetails(int id)
        {
            NewsCategory category = db.NewsCategories.Find(id);
            if (category == null)
                return RedirectToAction("CategoryIndex");
            return View("CategoryDetails", category);
        }

        // 
        // GET: /News/CategoryEdit
        [Authorize]
        public ActionResult CategoryEdit(int id)
        {
            NewsCategory category = db.NewsCategories.Find(id);
            if (category == null)
                return RedirectToAction("CategoryIndex");
            return View(category);
        }

        // 
        // POST: /News/CategoryEdit 
        [Authorize]
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
        [Authorize]
        public ActionResult CategoryDelete(int id)
        {
            NewsCategory category = db.NewsCategories.Find(id);
            if (category == null)
                return RedirectToAction("CategoryIndex");
            return View(category);
        }

        // 
        // POST: /News/CategoryDelete 
        [Authorize]
        [HttpPost]
        public RedirectToRouteResult CategoryDelete(int id, FormCollection collection)
        {
            NewsCategory category = db.NewsCategories.Find(id);
            db.NewsCategories.Remove(category);
            db.SaveChanges();

            return RedirectToAction("CategoryIndex");
        }

        #endregion NewsCategory

        #region NewsComment
        
        //
        // POST: /News/CommentCreate
        [Authorize]
        [HttpPost]
        public RedirectToRouteResult CommentCreate(NewsCommentViewModel model)
        {
            NewsComment comment = new NewsComment();
            comment.Author = Membership.GetUser().UserName;
            comment.Date = DateTime.Now;
            comment.Text = model.Text;
            if (ModelState.IsValid)
            {
                //int NewsID = Int32.Parse(values["NewsID"]);
                var news = db.News.Find(model.NewsID);
                news.NewsComments.Add(comment);
                db.SaveChanges();
            }
            else
            {
                ViewBag.CommentError = "Błąd! Komentarza nie dodano.";
            }
            return RedirectToAction("Details", new { ID = model.NewsID });
        }

        //
        // GET: /News/CommentEdit
        [Authorize]
        public ActionResult CommentEdit(int id)
        {
            var newsComment = db.NewsComments.Find(id);
            if (newsComment == null)
                return RedirectToAction("Index", "Home");
            if (!newsComment.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName,"admin"))
                return RedirectToAction("Details", new { ID = newsComment.News.ID });
            return View(newsComment);
        }

        //
        // POST: /News/CommentEdit
        [Authorize]
        [HttpPost]
        public ActionResult CommentEdit(FormCollection values)
        {
            int CommentID = Int32.Parse(values["ID"]);
            var newsComment = db.NewsComments.Find(CommentID);
            if(newsComment == null)
                return RedirectToAction("Index", "Home");
            if (!newsComment.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName, "admin"))
                return RedirectToAction("Details", new { ID = newsComment.News.ID });
            try
            {   
                newsComment.Date = DateTime.Now;
                newsComment.Text = values["Text"];
                UpdateModel(newsComment);
                db.SaveChanges();   
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Błąd edycji!");
            }
            return RedirectToAction("Details", new { ID = newsComment.News.ID }); 
        }

        // 
        // GET: /News/CommentDelete
        [Authorize]
        public ActionResult CommentDelete(int id)
        {
            var newsComment = db.NewsComments.Find(id);
            if (newsComment == null)
                return RedirectToAction("Index", "Home");
            if(!newsComment.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName, "admin"))
                return RedirectToAction("Details", new { ID = newsComment.News.ID });
            return View(newsComment);
        }

        // 
        // POST: /News/CommentDelete 
        [Authorize]
        [HttpPost]
        public RedirectToRouteResult CommentDelete(int id, FormCollection collection)
        {
            var newsComment = db.NewsComments.Find(id);
            if (newsComment == null)
                return RedirectToAction("Index", "Home");
            if(!newsComment.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName, "admin"))
                return RedirectToAction("Details", new { ID = newsComment.News.ID });                
            db.NewsComments.Remove(newsComment);
            db.SaveChanges();

            return RedirectToAction("Details", new { ID = newsComment.News_ID });
        }

        #endregion NewsComment
    }
}
