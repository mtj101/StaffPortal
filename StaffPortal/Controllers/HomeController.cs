using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using StaffPortal.Models;

namespace StaffPortal.Controllers
{
    [RoutePrefix("")]
    public class HomeController : Controller
    {
        private ApplicationUserManager _userManager;

        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
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

            var yearStart = new DateTime(DateTime.Now.Year,1,1);
            var yearEnd = new DateTime(DateTime.Now.Year + 1, 1, 1);

            user.HolidaysBooked =
                db.HolidayBooking
                .Where(h => h.StaffMember.Id == user.Id && h.Start >= yearStart && h.End < yearEnd && h.IsApproved)
                .AsEnumerable()
                .Aggregate(0, (total, d) => total + (int)(d.End - d.Start).TotalDays);

            user.HolidaysPending =
                db.HolidayBooking
                .Where(h => h.StaffMember.Id == user.Id && h.Start >= yearStart && h.End < yearEnd && !h.IsApproved)
                .AsEnumerable()
                .Aggregate(0, (total, d) => total + (int)(d.End - d.Start).TotalDays);         

            return View(user);
        }

        public class LoggedInStaffMember
        {
            public int Id { get; set; }
            public int HolidaysBooked { get; set; }
            public int HolidaysPending { get; set; }
            public int Sickness { get; set; }
        }
    }
}