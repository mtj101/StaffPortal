﻿using System;
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
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    StaffMember = new StaffMember
                    {
                        Surname = model.Surname,
                        FirstNames = model.FirstNames,
                        DepartmentId = model.DepartmentId,
                        Address1 = model.Address1,
                        Address2 = model.Address2,
                        City = model.City,
                        PostCode = model.PostCode,
                        County = model.County,
                        Country = model.Country,
                        PhoneNumber = model.PhoneNumber,
                        DateOfBirth = model.DateOfBirth,
                        StartDate = model.StartDate
                    }
                };

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
                Surname = staffMember.Surname,
                PhoneNumber = staffMember.PhoneNumber,
                Address1 = staffMember.Address1,
                Address2 = staffMember.Address2,
                City = staffMember.City,
                County = staffMember.County,
                PostCode = staffMember.PostCode,
                Country = staffMember.Country,
                Email = identityUser.Email
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("users/edit", Name = "saveuserchanges")]
        public ActionResult EditUser(EditMemberViewModel viewModel)
        {
            var db = new ApplicationDbContext();

            if (!ModelState.IsValid)
            {
                var departments = db.Department.Select(d => new SelectListItem { Text = d.Name }).ToList();
                var roles = db.Roles.Select(r => new SelectListItem { Text = r.Name }).ToList();
                viewModel.Departments = departments;
                viewModel.Roles = roles;

                return View(viewModel);
            }
            

            var staffMember = db.StaffMember.Single(s => s.Id == viewModel.Id);
            var identityUser = UserManager.FindById(db.Users.Single(u => u.StaffMemberId == staffMember.Id).Id);

            // change the role
            string currentRole = UserManager.GetRoles(identityUser.Id).Single();
            UserManager.RemoveFromRole(identityUser.Id, currentRole);
            UserManager.AddToRole(identityUser.Id, viewModel.RoleName);

            // change the username
            if (identityUser.Email != viewModel.Email)
            {
                identityUser.Email = viewModel.Email;
                identityUser.UserName = viewModel.Email;
                UserManager.Update(identityUser);
            }

            // change the department
            int chosenDepartmentId = db.Department.Single(d => d.Name == viewModel.DepartmentName).Id;
            staffMember.DepartmentId = chosenDepartmentId;

            // change personal details
            staffMember.FirstNames = viewModel.FirstNames;
            staffMember.Surname = viewModel.Surname;

            staffMember.Address1 = viewModel.Address1;
            staffMember.Address2 = viewModel.Address2;
            staffMember.City = viewModel.City;
            staffMember.PostCode = viewModel.PostCode;
            staffMember.County = viewModel.County;
            staffMember.Country = viewModel.Country;

            staffMember.PhoneNumber = viewModel.PhoneNumber;

            db.SaveChanges();

            return RedirectToAction("ViewUsers");
        }

        [Route("settings")]
        public ActionResult EditSettings()
        {
            var db = new ApplicationDbContext();
            var settings = db.ApplicationSettings.ToDictionary(s => s.SettingName, s => s.Value);

            var viewModel = new EditSettingsViewModel
            {
                Settings = settings
            };

            return View(viewModel);
        }


        [Route("settings")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSettings(EditSettingsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var db = new ApplicationDbContext();
                foreach (var setting in viewModel.Settings)
                {
                    db.ApplicationSettings.SingleOrDefault(s => s.SettingName == setting.Key).Value = setting.Value;
                }
                db.SaveChanges();
            }

            return View(viewModel);
        }
    }

    public class UserCreationViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
    }
}