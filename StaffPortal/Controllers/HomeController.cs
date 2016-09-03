using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using StaffPortal.Models;
using StaffPortal.Models.ViewModels;
using StaffPortal.Service;

namespace StaffPortal.Controllers
{
    [RoutePrefix("")]
    public class HomeController : Controller
    {
        private ApplicationUserManager _userManager;
        private BookingService _bookingService = new BookingService();
        private AlertService _alertService = new AlertService();

        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("calendar")]
        public async Task<ActionResult> Calendar()
        {
            var user = new LoggedInStaffMember();
            if (User.Identity.IsAuthenticated)
            {
                user.Id = (await UserManager.FindByNameAsync(User.Identity.Name)).StaffMemberId;
            }
            var db = new ApplicationDbContext();

            var totals = _bookingService.GetHolidayTotalsForUser(user.Id);
            user.HolidaysBooked = totals.Booked;
            user.HolidaysPending = totals.Pending;
            user.MaximumHolidays = int.Parse(db.ApplicationSettings.Find("Holidays Per Member").Value);

            return View(user);
        }

        [Authorize]
        [Route("userpanel")]
        public async Task<ActionResult> UserPanel()
        {
            var user = new LoggedInStaffMember();
            if (User.Identity.IsAuthenticated)
            {
                user.Id = (await UserManager.FindByNameAsync(User.Identity.Name)).StaffMemberId;
            }

            var totals = _bookingService.GetHolidayTotalsForUser(user.Id);
            user.HolidaysBooked = totals.Booked;
            user.HolidaysPending = totals.Pending;

            return View(user);
        }

        [ChildActionOnly]
        public ActionResult SideBar()
        {
            var viewModel = new SideBarViewModel();

            if (User.IsInRole("Admin"))
            {
                viewModel.PendingHolidaysForSupervisor = new SupervisorService().GetAllPendingHolidays().Count;
            }
            if (User.IsInRole("Supervisor"))
            {
                int userId = UserManager.FindById(User.Identity.GetUserId()).StaffMemberId;
                viewModel.PendingHolidaysForSupervisor = new SupervisorService().GetPendingHolidaysForSupervisor(userId).Count;
            }

            return View("_SideBar", viewModel);
        }

        [ChildActionOnly]
        public ActionResult GetAlerts()
        {
            var alerts = _alertService.GetAlertsForUser(User.Identity.GetUserId()).OrderByDescending(a => a.Created).ToList();
            return View("_GetAlerts", alerts);
        }
    }
}