using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using GrupaEka.Models;
using GrupaEka.Helpers;
using System.Text;
using System.Net.Mail;
using System.IO;

namespace GrupaEka.Controllers
{
    public class AccountController : Controller
    {
        private IGrupaEkaDB db;

        public AccountController()
        {
            db = new GrupaEkaDB();
        }

        public AccountController(IGrupaEkaDB dbContext)
        {
            db = dbContext;
        }

        #region CRUD
        //
        // GET: /Account/Index
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            var users = Membership.GetAllUsers();
            ViewBag.roles = getRolesToList();
            return View(users);
        }

        //
        // GET: /Account/Register

        [Authorize(Roles = "admin")]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.Profile.UserName, model.Password, model.Email, null, null, true, null, out createStatus);
                Roles.AddUserToRole(model.Profile.UserName, "user");

                //upload user photo
                foreach (string upload in Request.Files)
                {
                    if (Request.Files[upload].ContentLength==0) 
                        continue;
                    string path = AppDomain.CurrentDomain.BaseDirectory + "Content/profile-photos/";
                    string ext = Request.Files[upload].FileName.Substring(Request.Files[upload].FileName.LastIndexOf('.'));
                    model.Profile.Photo = model.Profile.UserName + ext;
                    Request.Files[upload].SaveAs(Path.Combine(path, model.Profile.Photo));
                }
                //end of upload user photo

                db.Profiles.Add(model.Profile);
                db.SaveChanges();
                if (createStatus == MembershipCreateStatus.Success)
                {
                    //FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Account");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // 
        // GET: /Account/Details

        [Authorize(Roles = "admin")]
        public ActionResult Details(string UserName)
        {
            var user = Membership.GetUser(UserName);
            if (user == null)
                return RedirectToAction("Index");
            ViewBag.Role = Roles.GetRolesForUser(UserName)[0];
            return View("Details", user);
        }

        // 
        // GET: /Account/Edit
        [Authorize(Roles = "admin")]
        public ActionResult Edit(string UserName)
        {
            var user = Membership.GetUser(UserName);
            if (user == null)
                return RedirectToAction("Index");
            ViewBag.Role = Roles.GetRolesForUser(UserName)[0];
            ViewBag.roles = getRolesToList();
            return View(user);
        }

        // 
        // POST: /Account/Edit 
        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Edit(UserRoleViewModel model)
        {
            try
            {
                MembershipUser user = Membership.GetUser(model.UserName,false);

                //change user name
                //if (model.newUserName != model.UserName)
                //{
                //    var pass = user.GetPassword();
                //    var newUser = Membership.CreateUser(model.newUserName, pass);
                //    newUser.IsApproved = user.IsApproved;
                //    newUser.Email = user.Email;
                //    Roles.RemoveUserFromRole(model.UserName, Roles.GetRolesForUser(model.UserName)[0]);
                //    Roles.AddUserToRole(model.newUserName, model.Role);
                //    Membership.DeleteUser(user.UserName);
                //    user = newUser;
                //}

                //change e-mail
                user.Email = model.Email;

                //change role
                if (Roles.GetRolesForUser(model.UserName)[0] != model.Role)
                {
                    Roles.RemoveUserFromRole(model.UserName, Roles.GetRolesForUser(model.UserName)[0]);
                    Roles.AddUserToRole(model.UserName, model.Role);
                }
                
                //change approvement                
                user.IsApproved = model.IsApproved;
                
                Membership.UpdateUser(user);
                

                if (Request.IsAjaxRequest())
                    return Content("Success");
                else
                    return RedirectToAction("Details", new { UserName = user.UserName });
                    //return Redirect(Request.UrlReferrer.ToString());

            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Edit Failure, see inner exception");
            }

            //return View(model);
            return RedirectToAction("Index");
        }

        // 
        // GET: /Account/Delete
        [Authorize(Roles = "admin")]
        public ActionResult Delete(string UserName)
        {
            var user = Membership.GetUser(UserName);
            if (user == null)
                return RedirectToAction("Index");
            ViewBag.Role = Roles.GetRolesForUser(UserName)[0];
            return View(user);
        }

        // 
        // POST: /Account/Delete 
        [Authorize(Roles = "admin")]
        [HttpPost]
        public RedirectToRouteResult Delete(string UserName, FormCollection form)
        {
            var profile = db.Profiles.Where(p => p.UserName == UserName).SingleOrDefault();
            db.Profiles.Remove(profile);
            db.SaveChanges();
            Roles.RemoveUserFromRole(UserName, Roles.GetRolesForUser(UserName)[0]);
            Membership.DeleteUser(UserName);
            return RedirectToAction("Index");
        }



        #endregion CRUD

        #region LogOn

        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult LogOnFast()
        {
            return PartialView();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Błędna nazwa użytkownika lub hasło.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        #endregion LogOn

        #region ChangePassword

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "Obecne hasło lub nowe jest niepoprawne.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        [Authorize]
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #endregion ChangePassword

        #region ResetPassword

        //
        // GET: /Account/ResetPassword
        public ActionResult ResetPassword()
        {
            return View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        public ActionResult ResetPassword(string email)
        {

            string user = Membership.GetUserNameByEmail(email);
            if (user == null)
            {
                ViewBag.ErrorMsg = "Nie znaleziono użytkownika o podanym adresie e-mail.";
                return View("ResetPasswordFail");
            }
            
            //generate reset password code
            Random random = new Random((int)DateTime.Now.Ticks);
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < 32; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            string token = builder.ToString();
            //--end of generate reset password code

            var pr = db.PasswordResets.Where(p => p.UserName == user).SingleOrDefault();
            if (pr == null)
            {
                db.PasswordResets.Add(new PasswordReset { UserName = user, Token = token });
            }
            else
            {
                pr.Token = token;
            }
            db.SaveChanges();

            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("noreply@grupaeka.studentlive.pl", "Grupa .NET EKA");
                msg.To.Add(email);
                msg.Subject = "Grupa .NET EKA: Resetowanie hasła";
                msg.Body = "Aby zresetować hasło wejdź na poniższy link:"+
                    "<br />http://" + Request.Url.Host + "/Account/ResetPasswordConfirm?UserName="+user+"&Token="+token;
                msg.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.live.com", 25);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("jj09@studentlive.pl", "master");
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(msg);

                msg.Dispose();

                return View("ResetPasswordSent");
            }
            catch (Exception)
            {
            }

            ViewBag.ErrorMsg = "Przepraszamy. Nie można wysłać e-maila z kodem resetującym hasło.";
            return View("ResetPasswordFail");
        }

        //
        // GET: /Account/ResetPasswordConfirm
        public ActionResult ResetPasswordConfirm(string UserName, string Token)
        {
            var rp = db.PasswordResets.Where(r => r.UserName == UserName && r.Token == Token).SingleOrDefault();
            if (rp == null)
            {
                ViewBag.ErrorMsg = "Nie poprawna nazwa użytkownika lub token. Spróbuj jeszcze raz.";
                return View("ResetPasswordFail");
            }

            var user = Membership.GetUser(UserName);
            string newPassword = user.ResetPassword();
            Membership.UpdateUser(user);
            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("noreply@grupaeka.studentlive.pl", "Grupa .NET EKA");
                msg.To.Add(user.Email);
                msg.Subject = "Grupa .NET EKA: Resetowanie hasła";
                msg.Body = "Twoje nowe hasło to: " + newPassword;
                msg.IsBodyHtml = false;

                SmtpClient smtp = new SmtpClient("smtp.live.com", 25);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("jj09@studentlive.pl", "master");
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(msg);

                msg.Dispose();

                db.PasswordResets.Remove(rp);
                db.SaveChanges();
                

                return View("ResetPasswordSuccess");
            }
            catch (Exception)
            {
            }

            ViewBag.ErrorMsg = "Przepraszamy. Nie można wysłać e-maila z nowym hasłem. Spróbuj ponownie.";
            return View("ResetPasswordFail");
        }

        //
        // GET: /Account/ResetPasswordFail
        public ActionResult ResetPasswordFail()
        {
            return View();
        }

        //
        // GET: /Account/ResetPasswordSuccess
        public ActionResult ResetPasswordSuccess()
        {
            return View();
        }

        //
        // GET: /Account/ResetPasswordSent
        public ActionResult ResetPasswordSent()
        {
            return View();
        }

        #endregion ResetPassword

        #region ChangeEmail

        //
        // GET: /Account/ChangeEmail

        [Authorize]
        public ActionResult ChangeEmail()
        {
            return View();
        }

        //
        // POST: /Account/ChangeEmail

        [Authorize]
        [HttpPost]
        public ActionResult ChangeEmail(ChangeEmailModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changeEmailSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    currentUser.Email = model.NewEmail;
                    Membership.UpdateUser(currentUser);
                    changeEmailSucceeded = true;
                }
                catch (Exception)
                {
                    changeEmailSucceeded = false;
                }

                if (changeEmailSucceeded)
                {
                    return RedirectToAction("ChangeEmailSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "Zmiana adresu e-mail się nie powiodła.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangeEmailSuccess

        [Authorize]
        public ActionResult ChangeEmailSuccess()
        {
            return View();
        }

        #endregion ChangeEmail

        #region Helpers

        private List<SelectListItem> getRolesToList()
        {
            List<SelectListItem> roles = new List<SelectListItem>();
            var allRoles = Roles.GetAllRoles();
            foreach (var role in allRoles)
            {
                roles.Add(new SelectListItem
                {
                    Text = role,
                    Value = role
                });
            }
            return roles;
        }

        #endregion Helpers

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
