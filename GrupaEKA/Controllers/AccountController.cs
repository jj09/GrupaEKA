using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using GrupaEka.Models;

namespace GrupaEka.Controllers
{
    public class AccountController : Controller
    {
        public List<SelectListItem> getRolesToList()
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
                    return Redirect(Request.UrlReferrer.ToString());

            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Edit Failure, see inner exception");
            }

            //return View(model);
            return RedirectToAction("Index");
        }

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
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
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

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);
                Roles.AddUserToRole(model.UserName, "user");
                if (createStatus == MembershipCreateStatus.Success)
                {
                    //FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
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
        public ActionResult Details(string UserName)
        {
            var user = Membership.GetUser(UserName);
            if (user == null)
                return RedirectToAction("Index");
            ViewBag.Role = Roles.GetRolesForUser(UserName)[0];
            return View("Details", user);
        }

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

        public ActionResult ChangePasswordSuccess()
        {
            return View();
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
            Roles.RemoveUserFromRole(UserName, Roles.GetRolesForUser(UserName)[0]);
            Membership.DeleteUser(UserName);
            return RedirectToAction("Index");
        }

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
