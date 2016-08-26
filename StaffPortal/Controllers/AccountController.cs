using System;
using System.Data.Entity;
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
using StaffPortal.Models.ViewModels;

namespace StaffPortal.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public ApplicationSignInManager SignInManager => HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
        public ApplicationUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
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
                var staffMember = db.StaffMember.Include(s => s.Department).SingleOrDefault(d => d.Id == userResult.Result.StaffMemberId);

                var viewModel = new LoginPartialViewModel
                {
                    Id = userResult.Result.Id,
                    Name = staffMember.FirstNames + " " + staffMember.Surname,
                    Department = staffMember.Department.Name
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
        public string Department { get; set; }

    }
}