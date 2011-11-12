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
    public class LectureController : Controller
    {
        private GrupaEkaDB db = new GrupaEkaDB();

        #region Lecture

        //
        // GET: /Lecture/
        public ActionResult Index(int start = 1)
        {
            if (start < 1)
                start = 1;

            IQueryable<Lecture> lectures = db.Lectures.Where(n => n.Date <= DateTime.Now).OrderByDescending(n => n.Date).Skip(start - 1);

            int lecturesCount = lectures.Count();
            ViewBag.PagesCount = (lecturesCount + start - 1) / 10 + (lecturesCount % 10 > 0 ? 1 : 0);
            ViewBag.CurrentPage = ((start - 1) / 10) + 1;

            return View(lectures.Take(10));
        }

        //
        // GET: /Lecture/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.Now = DateTime.Now;
            LectureTimeViewModel newLectureTime = new LectureTimeViewModel();
            newLectureTime.Lecture = new Lecture();
            newLectureTime.Lecture.Date = DateTime.Now;
            return View(newLectureTime);
        }

        //
        // POST: /Lecture/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(LectureTimeViewModel newLectureTime)
        {
            if (ModelState.IsValid)
            {
                Lecture newLecture = newLectureTime.Lecture;
                int hour, minutes;
                Int32.TryParse(newLectureTime.Hour, out hour);
                Int32.TryParse(newLectureTime.Minutes, out minutes);
                TimeSpan time = new TimeSpan(hour, minutes, 0);
                newLecture.Date = newLecture.Date + time;

                //add file
                foreach (string upload in Request.Files)
                {
                    if (Request.Files[upload].ContentLength == 0)
                        continue;

                    string path = AppDomain.CurrentDomain.BaseDirectory + "Content/files/";

                    string ext = Request.Files[upload].FileName.Substring(Request.Files[upload].FileName.LastIndexOf('.'));
                    string baseName = Request.Files[upload].FileName.Substring(0,Request.Files[upload].FileName.LastIndexOf('.'));
                    string uploadDate = String.Format("({0:d})", newLecture.Date);
                    newLecture.File = (baseName + uploadDate).Replace(' ', '_').Replace('/', '_') + ext;
                    Request.Files[upload].SaveAs(Path.Combine(path, newLecture.File));
                }
                //end add file

                db.Lectures.Add(newLecture);
                db.SaveChanges();

                return RedirectToAction("Index", "Lecture");
            }
            else
            {
                ViewBag.Now = DateTime.Now;
                ViewBag.NewsCategories = db.NewsCategories;
                return View(newLectureTime);
            }
        }

        // 
        // GET: /Lecture/Details
        public ActionResult Details(int id)
        {
            Lecture lecture = db.Lectures.Find(id);
            if (lecture == null)
                return RedirectToAction("Index", "Lecture");
            return View("Details", lecture);
        }

        // 
        // GET: /Lecture/Edit
        [Authorize]
        public ActionResult Edit(int id)
        {
            Lecture lecture = db.Lectures.Find(id);
            if (lecture == null)
                return RedirectToAction("Index", "Lecture");
            if (!lecture.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName, "admin"))
                return RedirectToAction("Details", new { ID = lecture.ID });
            LectureTimeViewModel editLecture = new LectureTimeViewModel();
            editLecture.Lecture = lecture;
            editLecture.Hour = lecture.Date.Hour.ToString();
            editLecture.Minutes = lecture.Date.Minute.ToString();
            return View(editLecture);
        }

        // 
        // POST: /Lecture/Edit 
        [Authorize]
        [HttpPost]
        public ActionResult Edit(LectureTimeViewModel model)
        {
            try
            {
                var lecture = db.Lectures.Find(model.Lecture.ID);
                if (lecture == null)
                    return RedirectToAction("Index", "Lecture");
                if (!lecture.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName, "admin"))
                    return RedirectToAction("Details", new { ID = lecture.ID });
                int hour, minutes;
                Int32.TryParse(model.Hour, out hour);
                Int32.TryParse(model.Minutes, out minutes);
                TimeSpan time = new TimeSpan(hour, minutes, 0);
                lecture.Date = model.Lecture.Date + time;
                lecture.Text = model.Lecture.Text;
                lecture.Title = model.Lecture.Title;

                //add file
                foreach (string upload in Request.Files)
                {
                    if (Request.Files[upload].ContentLength == 0)
                        continue;

                    string path = AppDomain.CurrentDomain.BaseDirectory + "Content/files/";

                    FileInfo TheFile = new FileInfo(path + model.Lecture.File);
                    if (TheFile.Exists)
                    {
                        TheFile.Delete();
                    }

                    string ext = Request.Files[upload].FileName.Substring(Request.Files[upload].FileName.LastIndexOf('.'));
                    string baseName = Request.Files[upload].FileName.Substring(0,Request.Files[upload].FileName.LastIndexOf('.'));
                    string uploadDate = String.Format("({0:d})", lecture.Date);
                    lecture.File = (baseName+uploadDate).Replace(' ', '_').Replace('/', '_') + ext;
                    Request.Files[upload].SaveAs(Path.Combine(path, lecture.File));
                }
                //end add file

                UpdateModel(lecture);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = model.Lecture.ID });
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Błąd edycji!");
            }
            return View(model);
        }

        // 
        // GET: /Lecture/Delete
        [Authorize]
        public ActionResult Delete(int id)
        {
            Lecture lecture = db.Lectures.Find(id);
            if (lecture == null)
                return RedirectToAction("Index", "Lecture");
            if (!lecture.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName, "admin"))
                return RedirectToAction("Details", new { ID = lecture.ID });
            return View(lecture);
        }

        // 
        // POST: /Lecture/Delete 
        [Authorize]
        [HttpPost]
        public RedirectToRouteResult Delete(int id, FormCollection collection)
        {
            Lecture lecture = db.Lectures.Find(id);

            if (lecture == null)
                return RedirectToAction("Index", "Lecture");
            if (!lecture.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName, "admin"))
                return RedirectToAction("Details", new { ID = lecture.ID });

            //delete file
            string path = AppDomain.CurrentDomain.BaseDirectory + "Content/files/";
            FileInfo TheFile = new FileInfo(path + lecture.File);
            if (TheFile.Exists)
            {
                TheFile.Delete();
            }
            //end delete file

            db.Lectures.Remove(lecture);
            db.SaveChanges();

            return RedirectToAction("Index", "Lecture");
        }

        #endregion Lecture
        
        #region LectureComment

        //
        // POST: /Lecture/CommentCreate
        [Authorize]
        [HttpPost]
        public RedirectToRouteResult CommentCreate(LectureCommentViewModel model)
        {
            LectureComment comment = new LectureComment();
            comment.Author = Membership.GetUser().UserName;
            comment.Date = DateTime.Now;
            comment.Text = model.Text;
            if (ModelState.IsValid)
            {
                var lecture = db.Lectures.Find(model.LectureID);
                lecture.LectureComments.Add(comment);
                db.SaveChanges();
            }
            else
            {
                ViewBag.CommentError = "Błąd! Komentarza nie dodano.";
            }
            return RedirectToAction("Details", new { ID = model.LectureID });
        }

        //
        // GET: /Lecture/CommentEdit
        [Authorize]
        public ActionResult CommentEdit(int id)
        {
            var lectureComment = db.LectureComments.Find(id);
            if (lectureComment == null)
                return RedirectToAction("Index", "Lecture");
            if (!lectureComment.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName, "admin"))
                return RedirectToAction("Details", new { ID = lectureComment.Lecture.ID });
            return View(lectureComment);
        }

        //
        // POST: /Lecture/CommentEdit
        [Authorize]
        [HttpPost]
        public ActionResult CommentEdit(FormCollection values)
        {
            int CommentID = Int32.Parse(values["ID"]);
            var lectureComment = db.LectureComments.Find(CommentID);
            if (lectureComment == null)
                return RedirectToAction("Index", "Lecture");
            if (!lectureComment.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName, "admin"))
                return RedirectToAction("Details", new { ID = lectureComment.Lecture.ID });
            try
            {
                lectureComment.Date = DateTime.Now;
                lectureComment.Text = values["Text"];
                UpdateModel(lectureComment);
                db.SaveChanges();
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Błąd edycji!");
            }
            return RedirectToAction("Details", new { ID = lectureComment.Lecture.ID });
        }

        // 
        // GET: /Lecture/CommentDelete
        [Authorize]
        public ActionResult CommentDelete(int id)
        {
            var lectureComment = db.LectureComments.Find(id);
            if (lectureComment == null)
                return RedirectToAction("Index", "Lecture");
            if (!lectureComment.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName, "admin"))
                return RedirectToAction("Details", new { ID = lectureComment.Lecture.ID });
            return View(lectureComment);
        }

        // 
        // POST: /Lecture/CommentDelete 
        [Authorize]
        [HttpPost]
        public RedirectToRouteResult CommentDelete(int id, FormCollection collection)
        {
            var lectureComment = db.LectureComments.Find(id);
            if (lectureComment == null)
                return RedirectToAction("Index", "Lecture");
            if (!lectureComment.Author.Equals(Membership.GetUser().UserName) && !Roles.IsUserInRole(Membership.GetUser().UserName, "admin"))
                return RedirectToAction("Details", new { ID = lectureComment.Lecture.ID });
            db.LectureComments.Remove(lectureComment);
            db.SaveChanges();

            return RedirectToAction("Details", new { ID = lectureComment.Lecture_ID });
        }

        #endregion LectureComment
    }
}
