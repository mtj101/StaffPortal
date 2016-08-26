using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using StaffPortal.Business;
using StaffPortal.Models;
using StaffPortal.Models.ViewModels;

namespace StaffPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("admin")]
    public class AdminController : Controller
    {
        #region identity

        public ApplicationSignInManager SignInManager => HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
        public ApplicationUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        #endregion

        [Route("users/add")]
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

        [Route("users/add")]
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

        [Route("users/edit")]
        public ActionResult ViewUsers()
        {
            var db = new ApplicationDbContext();

            var staffMembers = db.StaffMember.Include(s => s.Department).ToList();
            var users = new List<ViewMembersViewModel>();

            foreach (var staffMember in staffMembers)
            {
                var identityUser = db.Users.SingleOrDefault(u => u.StaffMemberId == staffMember.Id);
                string currentRole = UserManager.GetRoles(identityUser.Id).First();
                users.Add(new ViewMembersViewModel
                {
                    Id = staffMember.Id,
                    Name = $"{staffMember.FirstNames} {staffMember.Surname}",
                    Department = staffMember.Department.Name,
                    Role = currentRole
                });
            }

            return View(users);
        }

        [Route("users/edit/{memberId:int}", Name = "edituser")]
        public ActionResult EditUser(int memberId)
        {
            var db = new ApplicationDbContext();

            var departments = db.Department.Select(d => new SelectListItem { Text = d.Name }).ToList();
            var roles = db.Roles.Select(r => new SelectListItem { Text = r.Name }).ToList();

            var identityUser = db.Users.SingleOrDefault(u => u.StaffMemberId == memberId);

            if (identityUser == null)
            {
                return RedirectToAction("ViewUsers");
            }

            string currentRole = UserManager.GetRoles(identityUser.Id).First(); // only 1 role per user
            var staffMember = db.StaffMember.Include(s => s.Department).Single(s => s.Id == memberId);
            string currentDepartment = staffMember.Department.Name;

            var viewModel = new EditMemberViewModel
            {
                Id = staffMember.Id,
                Departments = departments,
                Roles = roles,
                RoleName = currentRole,
                DepartmentName = currentDepartment,
                FirstNames = staffMember.FirstNames,
                Surname = staffMember.Surname
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("users/edit", Name = "saveuserchanges")]
        public ActionResult EditUser(EditMemberViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var db = new ApplicationDbContext();

            var staffMember = db.StaffMember.Single(s => s.Id == viewModel.Id);
            var identityUser = db.Users.Single(u => u.StaffMemberId == staffMember.Id);

            // change the role
            string currentRole = UserManager.GetRoles(identityUser.Id).Single();
            UserManager.RemoveFromRole(identityUser.Id, currentRole);
            UserManager.AddToRole(identityUser.Id, viewModel.RoleName);

            // change the department and name
            int chosenDepartmentId = db.Department.Single(d => d.Name == viewModel.DepartmentName).Id;
            staffMember.DepartmentId = chosenDepartmentId;
            staffMember.FirstNames = viewModel.FirstNames;
            staffMember.Surname = viewModel.Surname;
            db.SaveChanges();

            return RedirectToAction("ViewUsers");
        }
    }

    public class UserCreationViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
    }
}