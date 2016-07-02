using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using StaffPortal.Models;
using StaffPortal.Service;

namespace StaffPortal.Controllers
{
    [RoutePrefix("")]
    public class HomeController : Controller
    {
        private ApplicationUserManager _userManager;
        private BookingService _bookingService => new BookingService();

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

            var totals = _bookingService.GetHolidayTotalsForUser(user.Id);
            user.HolidaysBooked = totals.Booked;
            user.HolidaysPending = totals.Pending;

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
    }
}