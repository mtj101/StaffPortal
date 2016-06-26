using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using StaffPortal.Models;

namespace StaffPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("admin")]
    public class AdminController : Controller
    {
        #region identity

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AdminController()
        {
        }

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
            private set { _signInManager = value; }
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        #endregion

        [Route("adduser")]
        public ActionResult AddNewUser()
        {
            var db = new ApplicationDbContext();

            var departments = db.Department.Select(d => new SelectListItem { Text = d.Name, Value = d.Id.ToString() }).ToList();
            var roles = db.Roles.Select(r => new SelectListItem {Text = r.Name}).ToList();

            var viewModel = new RegisterViewModel
            {
                Departments = departments,
                Roles = roles
            };

            return View(viewModel);
        }

        [Route("adduser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddNewUser(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var db = new ApplicationDbContext();
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, StaffMember = new StaffMember { Surname = model.Surname, FirstNames = model.FirstNames, DepartmentId = model.DepartmentId } };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, model.RoleName);
                    var newUser = new UserCreationViewModel
                    {
                        Name = $"{model.FirstNames} {model.Surname}",
                        Email = model.Email,
                        Department = model.RoleName
                    };
                    return RedirectToAction("ConfirmNewUser", newUser);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            return View(model);
        }

        [Route("useraddedconfirmation")]
        public ActionResult ConfirmNewUser(UserCreationViewModel creationViewModel)
        {          
            return View(creationViewModel);
        }
    }

    public class UserCreationViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
    }
}