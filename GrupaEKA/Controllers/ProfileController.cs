﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GrupaEka.Models;
using System.Web.Security;
using System.Net.Mail;
using System.IO;

namespace GrupaEka.Controllers
{ 
    public class ProfileController : Controller
    {
        private GrupaEkaDB db = new GrupaEkaDB();

        //
        // GET: /Profile/

        public ViewResult Index()
        {
            var profiles = db.Profiles.OrderBy(p => p.UserName);
            return View(profiles);
        }

        //
        // GET: /Profile/Details/5

        public ActionResult Details(int id)
        {
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
                return RedirectToAction("Index");
            ViewBag.Role = Roles.GetRolesForUser(profile.UserName)[0];
            return View(profile);
        }

        public ActionResult UserDetails(string UserName)
        {
            Profile profile = db.Profiles.Where(p=>p.UserName == UserName).SingleOrDefault();
            if (profile == null)
                return RedirectToAction("Index");
            ViewBag.Role = Roles.GetRolesForUser(profile.UserName)[0];
            return View("Details",profile);
        }
                
        //
        // GET: /Profile/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int id)
        {
            ViewBag.NewsCategories = db.NewsCategories;
            Profile profile = db.Profiles.Find(id);
            if (profile == null)
                return Redirect("Index");
            ViewBag.CurrentSubscriptions = profile.Subscriptions;
            return View(profile);
        }

        //
        // POST: /Profile/Edit/5

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Edit(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var profile = db.Profiles.Find(model.Profile.ID);
                if (profile == null)
                    return RedirectToAction("Index");

                //update fileds
                profile.FirstName = model.Profile.FirstName;
                profile.LastName = model.Profile.LastName;
                profile.About = model.Profile.About;
                profile.Faculty = model.Profile.Faculty;
                profile.StudyMajor = model.Profile.StudyMajor;
                profile.StudyYear = model.Profile.StudyYear;                

                //upload user photo
                foreach (string upload in Request.Files)
                {
                    if (Request.Files[upload].ContentLength == 0)
                        continue;

                    string path = AppDomain.CurrentDomain.BaseDirectory + "Content/profile-photos/";

                    FileInfo TheFile = new FileInfo(path + model.Profile.Photo);
                    if (TheFile.Exists)
                    {
                        TheFile.Delete();
                    }

                    string ext = Request.Files[upload].FileName.Substring(Request.Files[upload].FileName.LastIndexOf('.'));
                    profile.Photo = model.Profile.UserName + ext;
                    Request.Files[upload].SaveAs(Path.Combine(path, profile.Photo));
                }
                //end of upload user photo

                try
                {
                    var cats = db.NewsCategories.Where(r => model.CategoryIDs.Contains(r.ID));
                    profile.Subscriptions.Clear();
                    foreach (var cat in cats)
                    {
                        profile.Subscriptions.Add(cat);
                    }
                }
                catch (NotSupportedException) { /* 0 categories */ }

                UpdateModel(profile);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = model.Profile.ID });
            }
            return View(model);
        }

        //
        // GET: /Profile/MyProfile/5
        [Authorize]
        public ActionResult My()
        {
            ViewBag.NewsCategories = db.NewsCategories;
            string userName = Membership.GetUser().UserName;
            Profile profile = db.Profiles.Where(u => u.UserName == userName).SingleOrDefault();
            if(profile==null)
                return RedirectToAction("Index");
            ViewBag.CurrentSubscriptions = profile.Subscriptions;
            return View(profile);
        }

        //
        // POST: /Profile/MyProfile/5

        [Authorize]
        [HttpPost]
        public ActionResult My(ProfileViewModel model)
        {
            Edit(model);
            return RedirectToAction("Details", new { id = model.Profile.ID });
        }

        [HttpPost]
        [Authorize]
        public ActionResult SendEmail2User(Email email)
        {
                try
                {
                    MailMessage msg = new MailMessage();
                    var fromEmail = Membership.GetUser(email.From).Email;
                    msg.From = new MailAddress(fromEmail, email.From);
                    msg.Sender = new MailAddress(fromEmail, email.From);
                    msg.To.Add(email.EmailAddress);
                    msg.Subject = "Wiadomość od użytkownika Grupa .NET EKA";
                    msg.Body = "Nadawca: " + email.From + "\n" + "Email: " + fromEmail + "\n" + email.Message;
                    msg.IsBodyHtml = false;

                    SmtpClient smtp = new SmtpClient("smtp.live.com", 25);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential("jj09@studentlive.pl", "master");
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(msg);

                    msg.Dispose();

                    if (Request.IsAjaxRequest())
                        return Content("<script>alert('Twoja wiadomość została wysłana.');</script>");
                    else
                        return Redirect(Request.UrlReferrer.ToString());
                }
                catch (Exception)
                {
                }

                // If we are here...something kicked us into the exception.
                //
                if (Request.IsAjaxRequest())
                    return Content("<script>alert('BŁĄD! Twoja wiadomość NIE została wysłana.');</script>");
                else
                    return Redirect(Request.UrlReferrer.ToString());
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}