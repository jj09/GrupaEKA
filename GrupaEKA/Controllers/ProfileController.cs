using System;
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
            Profile profile = db.Profiles.Find(id);
            return View(profile);
        }

        //
        // POST: /Profile/Edit/5

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Edit(Profile profile)
        {
            if (ModelState.IsValid)
            {
                //upload user photo
                foreach (string upload in Request.Files)
                {
                    if (Request.Files[upload].ContentLength == 0)
                        continue;

                    string path = AppDomain.CurrentDomain.BaseDirectory + "Content/profile-photos/";

                    FileInfo TheFile = new FileInfo(path + profile.Photo);
                    if (TheFile.Exists)
                    {
                        TheFile.Delete();
                    }

                    string ext = Request.Files[upload].FileName.Substring(Request.Files[upload].FileName.LastIndexOf('.'));
                    profile.Photo = profile.UserName + ext;
                    Request.Files[upload].SaveAs(Path.Combine(path, profile.Photo));
                }
                //end of upload user photo

                db.Entry(profile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = profile.ID });
            }
            return View(profile);
        }

        //
        // GET: /Profile/MyProfile/5
        [Authorize]
        public ActionResult My()
        {
            string userName = Membership.GetUser().UserName;
            Profile profile = db.Profiles.Where(u => u.UserName == userName).SingleOrDefault();
            if(profile!=null)
                return View(profile);
            return RedirectToAction("Index");
        }

        //
        // POST: /Profile/MyProfile/5

        [Authorize]
        [HttpPost]
        public ActionResult My(Profile profile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(profile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = profile.ID });
            }
            return View(profile);
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