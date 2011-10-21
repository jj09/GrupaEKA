using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GrupaEka.Models;

namespace GrupaEka.Controllers
{
    public class HomeController : Controller
    {
        GrupaEkaDB db = new GrupaEkaDB();

        public ActionResult Index()
        {
            var news = db.News.Where(n => n.Date <= DateTime.Now).OrderByDescending(n => n.Date);

            return View(news);
        }

        public ActionResult Index2()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
