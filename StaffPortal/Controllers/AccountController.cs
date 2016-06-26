using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
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
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

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

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, true, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "Home");
                default:
                    ViewBag.ErrorMessage = "Could not log in. User doesn't exist or password was incorrect";
                    return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }


    }

    public class LoginPartialViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}