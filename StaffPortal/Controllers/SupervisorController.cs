using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using StaffPortal.Business;
using StaffPortal.Service;
using System.Linq;
using StaffPortal.Models.ViewModels;

namespace StaffPortal.Controllers
{
    [Authorize(Roles = "Supervisor, Admin")]
    [RoutePrefix("supervisor")]
    public class SupervisorController : Controller
    {

        public ApplicationUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        private readonly SupervisorService SupervisorService = new SupervisorService();
        private readonly StaffService StaffService = new StaffService();
        private readonly AlertService alertService = new AlertService();

        [Route("pending")]
        public ActionResult Pending()
        {
            List<HolidayBooking> pendingHolidays;
            if (User.IsInRole("Supervisor"))
            {
                int userId = UserManager.FindById(User.Identity.GetUserId()).StaffMemberId;
                pendingHolidays = SupervisorService.GetPendingHolidaysForSupervisor(userId);
            }
            else // Admin can see all
            {
                pendingHolidays = SupervisorService.GetAllPendingHolidays();
            }

            var viewModel = new SupervisorApprovalViewModel
            {
                SupervisorApprovals = pendingHolidays.Select(h => new SupervisorApprovalViewModel.SupervisorApproval
                {
                    HolidayBookingId = h.Id,
                    Name = $"{h.StaffMember.FirstNames} {h.StaffMember.Surname}",
                    Department = h.StaffMember.Department.Name,
                    Start = h.Start.ToShortDateString(),
                    End = h.End.ToShortDateString(),
                    IsApproved = null
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ApproveHoliday(SupervisorApprovalViewModel viewModel)
        {
            if (ModelState.IsValid)//approve != null)
            {
                var bookingService = new BookingService();

                // approve 
                var approvedHolidays = viewModel.SupervisorApprovals.Where(b => b.IsApproved == true).Select(b => b.HolidayBookingId).ToArray();
                alertService.SendHolidayApprovalAlert(approvedHolidays);
                bookingService.ApproveHolidays(approvedHolidays);
                

                // deny
                var deniedHolidays = viewModel.SupervisorApprovals.Where(b => b.IsApproved == false).Select(b => b.HolidayBookingId).ToArray();
                alertService.sendHolidayDenialAlert(deniedHolidays);
                bookingService.DenyHolidays(deniedHolidays);
                           
            }

            return RedirectToAction("Pending");
        }

        [Route("sickness")]
        public ActionResult Sickness()
        {
            int supervisorId = UserManager.FindById(User.Identity.GetUserId()).StaffMemberId;

            var staffForSupervisor = SupervisorService.GetStaffForSupervisor(supervisorId, User.IsInRole("Admin"));
            var sicknesses = SupervisorService.GetSicknessesForStaff(staffForSupervisor);

            var staffSicknesses = staffForSupervisor
                .ToDictionary(staff => staff, staff => sicknesses.Where(sickness => sickness.StaffMember == staff).ToList());

            var viewModel = new SicknessViewModel
            {
                StaffSicknesses = staffSicknesses
            };

            return View(viewModel);
        }

        [Route("sickness/add/{memberId:int}", Name = "addSicknessForMember")]
        public ActionResult AddSickness(int memberId)
        {
            var viewModel = new AddSicknessViewModel
            {
                StaffId = memberId
            };

            return View(viewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("sickness/add/{memberId:int}")]
        public ActionResult AddSickness(AddSicknessViewModel viewModel)
        {
            var staffMember = StaffService.GetStaffMemberById(viewModel.StaffId);

            if (viewModel.Start > viewModel.End)
            {
                ModelState.AddModelError("Start", "Start date must not be after end date");
            }

            if (ModelState.IsValid)
            {
                var sickness = new Sickness
                {
                    Start = viewModel.Start,
                    End = viewModel.End,
                    Reason = viewModel.Reason,
                    StaffMember = staffMember
                };

                SupervisorService.AddSickness(sickness);

                return RedirectToAction("Sickness");
            }

            return View(viewModel);


        }

    }

    public class SupervisorApprovalViewModel
    {
        public List<SupervisorApproval> SupervisorApprovals { get; set; }

        public class SupervisorApproval
        {
            public int HolidayBookingId { get; set; }
            public string Name { get; set; }
            public string Department { get; set; }
            public string Start { get; set; }
            public string End { get; set; }
            public bool? IsApproved { get; set; }
        }
    }


}