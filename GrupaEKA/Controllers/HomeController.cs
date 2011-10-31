using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GrupaEka.Models;
using System.Net.Mail;

namespace GrupaEka.Controllers
{
    public class HomeController : Controller
    {
        GrupaEkaDB db = new GrupaEkaDB();

        public ActionResult Index(string Category="", int start=1)
        {
            if (start < 1)
                start = 1;

            IQueryable<News> news;

            if (Category.Equals(String.Empty))
            {
                news = db.News.Where(n => n.Date <= DateTime.Now).OrderByDescending(n => n.Date).Skip(start - 1);
            }
            else
            {
                try
                {
                    var cat = db.NewsCategories.Where(c => c.Name == Category).Select(n=>n.Name).SingleOrDefault();    
                    news = db.News.Where(n => n.NewsCategories.Select(i => i.Name).Contains(cat)).Where(n => n.Date <= DateTime.Now).OrderByDescending(n => n.Date).Skip(start - 1);
                }
                catch (ArgumentNullException e)   //category not found
                {
                    news = db.News.Where(n => n.Date <= DateTime.Now).OrderByDescending(n => n.Date).Skip(start - 1);
                }
            }

            int newsCount = news.Count();
            ViewBag.PagesCount = newsCount / 10 + (newsCount % 10 > 0 ? 1 : 0);
            ViewBag.Category = Category;

            return View(news.Take(10));
        }

        public ActionResult About()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendEmail(Email email)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    MailMessage msg = new MailMessage();
                    msg.From = new MailAddress("noreply@grupaeka.studentlive.pl", "Grupa .NET EKA");
                    msg.To.Add("jjedrys@ksu.edu");
                    msg.Subject = "Wiadomość Grupa .NET EKA";
                    msg.Body = "Email: " + email.EmailAddress + "\n" + email.Message;
                    msg.IsBodyHtml = false;

                    SmtpClient smtp = new SmtpClient("smtp.live.com", 25);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential("jj09@studentlive.pl", "master");
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;                    
                    smtp.Send(msg);

                    msg.Dispose();

                    //ViewBag.Confirmation = "Twoja wiadomość została wysłana.";
                    if (Request.IsAjaxRequest())
                        return Content("Twoja wiadomość została wysłana.");
                    else
                        return Redirect(Request.UrlReferrer.ToString());

                    //return View("Index");
                }
                catch (Exception ex)
                {
                }

                // If we are here...something kicked us into the exception.
                //
                //ViewBag.Confirmation = "Twoja wiadomość nie została wysłana.";
                if (Request.IsAjaxRequest())
                    return Content("Twoja wiadomość NIE została wysłana.");
                else
                    return Redirect(Request.UrlReferrer.ToString());
                //return View("Index");
            }
            else
            {
                if (Request.IsAjaxRequest())
                    return Content("Twoja wiadomość NIE została wysłana.");
                else
                    return Redirect(Request.UrlReferrer.ToString());
            }
        }
    }
}
