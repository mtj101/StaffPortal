using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using StaffPortal.Business;
using StaffPortal.Service;

namespace StaffPortal.Controllers
{
    [Authorize(Roles = "Supervisor, Admin")]
    [RoutePrefix("supervisor")]
    public class SupervisorController : Controller
    {

        public ApplicationUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();


        [Route("pending")]
        public ActionResult Pending()
        {
            List<HolidayBooking> pendingHolidays;
            if (User.IsInRole("Supervisor"))
            {
                int userId = UserManager.FindById(User.Identity.GetUserId()).StaffMemberId;
                pendingHolidays = new SupervisorService().GetPendingHolidaysForSupervisor(userId);
            }
            else // Admin can see all
            {
                pendingHolidays = new SupervisorService().GetAllPendingHolidays();
            }

            return View(pendingHolidays);
        }

        [HttpPost]
        public ActionResult ApproveHoliday(int[] holidayIds)
        {
            var bookingService = new BookingService();
            bookingService.ApproveHolidays(holidayIds);
            return RedirectToAction("Pending");
        }


    }
}