using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using StaffPortal.Models;

namespace StaffPortal.Controllers
{
    [Authorize]
    public class Account2Controller : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public Account2Controller()
        {
        }

        public Account2Controller(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        [ChildActionOnly]
        [AllowAnonymous]
        public ActionResult LoginMenu()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userResult = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindByNameAsync(User.Identity.Name);
                userResult.Wait();

                var db = new ApplicationDbContext();
                var staffMember = db.StaffMember.SingleOrDefault(d => d.Id == userResult.Result.StaffMemberId);

                var viewModel = new LoginPartialViewModel
                {
                    Id = userResult.Result.Id,
                    Name = staffMember.FirstNames + " " + staffMember.Surname
                };
                return PartialView("_LoginPartial", viewModel);
            }           
            return PartialView("_LoginPartial");
        }


        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please retry with valid information";
                return View("Error");
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "Home");
                default:
                    ViewBag.ErrorMessage = "Could not log in. User doesn't exist or password was incorrect";
                    return View("Error");
            }
        }

        // GET: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var db = new ApplicationDbContext();
                var deparment = db.Department.SingleOrDefault(d => d.Name == model.DepartmentName);
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, StaffMember = new StaffMember { Surname = model.Surname, FirstNames = model.FirstNames, Department = deparment } };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Regular");
                    //UserManager.AddToRole(user.Id, "Admin");

                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
    }

    public class LoginPartialViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}