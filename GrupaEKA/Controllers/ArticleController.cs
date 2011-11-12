using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GrupaEka.Models;
using System.Data;
using System.Web.Security;
using System.Net.Mail;
using System.IO;

namespace GrupaEka.Controllers
{
    public class ArticleController : Controller
    {
        private GrupaEkaDB db = new GrupaEkaDB();

        #region Article

        //
        // GET: /Article/
        public ActionResult Index(int start = 1)
        {
            var articles = db.Articles.Where(n => n.Date <= DateTime.Now).OrderByDescending(n => n.Date);

            return View(articles);
        }

        //
        // GET: /Article/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.Now = DateTime.Now;
            ArticleTimeViewModel newArticleTime = new ArticleTimeViewModel();
            newArticleTime.Article = new Article();
            newArticleTime.Article.Date = DateTime.Now;
            return View(newArticleTime);
        }

        //
        // POST: /Article/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(ArticleTimeViewModel newArticleTime)
        {
            if (ModelState.IsValid)
            {
                Article newArticle = newArticleTime.Article;
                int hour, minutes;
                Int32.TryParse(newArticleTime.Hour, out hour);
                Int32.TryParse(newArticleTime.Minutes, out minutes);
                TimeSpan time = new TimeSpan(hour, minutes, 0);
                newArticle.Date = newArticle.Date + time;

                db.Articles.Add(newArticle);
                db.SaveChanges();

                return RedirectToAction("Index", "Article");
            }
            else
            {
                ViewBag.Now = DateTime.Now;
                ViewBag.NewsCategories = db.NewsCategories;
                return View(newArticleTime);
            }
        }

        // 
        // GET: /Article/Details
        public ActionResult Details(int id)
        {
            Article article = db.Articles.Find(id);
            if (article == null)
                return RedirectToAction("Index", "Article");
            return View("Details", article);
        }

        // 
        // GET: /Article/Edit
        [Authorize]
        public ActionResult Edit(int id)
        {
            Article article = db.Articles.Find(id);
            if (article == null)
                return RedirectToAction("Index", "Article");
            if (!article.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName, "admin"))
                return RedirectToAction("Details", new { ID = article.ID });
            ArticleTimeViewModel editArticle = new ArticleTimeViewModel();
            editArticle.Article = article;
            editArticle.Hour = article.Date.Hour.ToString();
            editArticle.Minutes = article.Date.Minute.ToString();
            return View(editArticle);
        }

        // 
        // POST: /Article/Edit 
        [Authorize]
        [HttpPost]
        public ActionResult Edit(ArticleTimeViewModel model)
        {
            try
            {
                var article = db.Articles.Find(model.Article.ID);
                if (article == null)
                    return RedirectToAction("Index", "Article");
                if (!article.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName, "admin"))
                    return RedirectToAction("Details", new { ID = article.ID });
                int hour, minutes;
                Int32.TryParse(model.Hour, out hour);
                Int32.TryParse(model.Minutes, out minutes);
                TimeSpan time = new TimeSpan(hour, minutes, 0);
                article.Date = model.Article.Date + time;
                article.Text = model.Article.Text;
                article.Title = model.Article.Title;

                UpdateModel(article);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = model.Article.ID });
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Błąd edycji!");
            }
            return View(model);
        }

        // 
        // GET: /Article/Delete
        [Authorize]
        public ActionResult Delete(int id)
        {
            Article article = db.Articles.Find(id);
            if (article == null)
                return RedirectToAction("Index", "Article");
            if (!article.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName, "admin"))
                return RedirectToAction("Details", new { ID = article.ID });
            return View(article);
        }

        // 
        // POST: /Article/Delete 
        [Authorize]
        [HttpPost]
        public RedirectToRouteResult Delete(int id, FormCollection collection)
        {
            Article article = db.Articles.Find(id);

            if (article == null)
                return RedirectToAction("Index", "Article");
            if (!article.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName, "admin"))
                return RedirectToAction("Details", new { ID = article.ID });

            db.Articles.Remove(article);
            db.SaveChanges();

            return RedirectToAction("Index", "Article");
        }

        #endregion Article

        #region ArticleComment

        //
        // POST: /Article/CommentCreate
        [Authorize]
        [HttpPost]
        public RedirectToRouteResult CommentCreate(ArticleCommentViewModel model)
        {
            ArticleComment comment = new ArticleComment();
            comment.Author = Membership.GetUser().UserName;
            comment.Date = DateTime.Now;
            comment.Text = model.Text;
            if (ModelState.IsValid)
            {
                var article = db.Articles.Find(model.ArticleID);
                article.ArticleComments.Add(comment);
                db.SaveChanges();
            }
            else
            {
                ViewBag.CommentError = "Błąd! Komentarza nie dodano.";
            }
            return RedirectToAction("Details", new { ID = model.ArticleID });
        }

        //
        // GET: /Article/CommentEdit
        [Authorize]
        public ActionResult CommentEdit(int id)
        {
            var articleComment = db.ArticleComments.Find(id);
            if (articleComment == null)
                return RedirectToAction("Index", "Article");
            if (!articleComment.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName, "admin"))
                return RedirectToAction("Details", new { ID = articleComment.Article.ID });
            return View(articleComment);
        }

        //
        // POST: /Article/CommentEdit
        [Authorize]
        [HttpPost]
        public ActionResult CommentEdit(FormCollection values)
        {
            int CommentID = Int32.Parse(values["ID"]);
            var articleComment = db.ArticleComments.Find(CommentID);
            if (articleComment == null)
                return RedirectToAction("Index", "Article");
            if (!articleComment.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName, "admin"))
                return RedirectToAction("Details", new { ID = articleComment.Article.ID });
            try
            {
                articleComment.Date = DateTime.Now;
                articleComment.Text = values["Text"];
                UpdateModel(articleComment);
                db.SaveChanges();
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Błąd edycji!");
            }
            return RedirectToAction("Details", new { ID = articleComment.Article.ID });
        }

        // 
        // GET: /Article/CommentDelete
        [Authorize]
        public ActionResult CommentDelete(int id)
        {
            var articleComment = db.ArticleComments.Find(id);
            if (articleComment == null)
                return RedirectToAction("Index", "Article");
            if (!articleComment.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName, "admin"))
                return RedirectToAction("Details", new { ID = articleComment.Article.ID });
            return View(articleComment);
        }

        // 
        // POST: /Article/CommentDelete 
        [Authorize]
        [HttpPost]
        public RedirectToRouteResult CommentDelete(int id, FormCollection collection)
        {
            var articleComment = db.ArticleComments.Find(id);
            if (articleComment == null)
                return RedirectToAction("Index", "Article");
            if (!articleComment.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName, "admin"))
                return RedirectToAction("Details", new { ID = articleComment.Article.ID });
            db.ArticleComments.Remove(articleComment);
            db.SaveChanges();

            return RedirectToAction("Details", new { ID = articleComment.Article_ID });
        }

        #endregion ArticleComment

    }
}
