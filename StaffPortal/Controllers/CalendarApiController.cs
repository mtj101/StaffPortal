using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using StaffPortal.Models;


namespace StaffPortal.Controllers
{
    [Authorize]
    [RoutePrefix("calendar")]
    public class CalendarApiController : ApiController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public CalendarApiController()
        {
        }

        public CalendarApiController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
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
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private IAuthenticationManager AuthenticationManager => HttpContext.Current.GetOwinContext().Authentication;

        [Route("bookeduserholidays")]
        [HttpGet]
        public IEnumerable<HolidayBooking> GetBookedHolidaysByStaffMember(DateTime start, DateTime end, int staffId)
        {
            var db = new ApplicationDbContext();
            var calEvts = db.HolidayBooking.Where(h => h.StaffMember.Id == staffId && h.IsApproved && h.Start >= start && h.End <= end).ToList();
            return calEvts;
        }

        [Route("pendinguserholidays")]
        [HttpGet]
        public IEnumerable<HolidayBooking> GetPendingHolidaysByStaffMember(DateTime start, DateTime end, int staffId)
        {
            var db = new ApplicationDbContext();
            var calEvts = db.HolidayBooking.Where(h => h.StaffMember.Id == staffId && !h.IsApproved && h.Start >= start && h.End <= end).ToList();
            return calEvts;
        }

        [AllowAnonymous]
        [Route("companyholidays")]
        [HttpGet]
        public IEnumerable<CompanyHoliday> GetCompanyHolidays(DateTime start, DateTime end)
        {
            var calEvts = new List<CompanyHoliday>{new CompanyHoliday()
            {
                Start = new DateTime(2016,3,28),
                End = new DateTime(2016,3,29),
                Title = "Easter"
            }
            };
            return calEvts;
        }

        [Authorize]
        [Route("bookholiday")]
        [HttpPost]
        public async Task<IHttpActionResult> BookHoliday(HolidayBooking requestedHoliday)
        {
            if (requestedHoliday.Start > requestedHoliday.End)
            {
                return BadRequest("Start date must be before end date");
            }

            int staffId = 0;
            if (User.Identity.IsAuthenticated)
            {
                staffId = (await UserManager.FindByNameAsync(User.Identity.Name)).StaffMemberId;
            }
            if (staffId != 0 && ModelState.IsValid)
            {
                var db = new ApplicationDbContext();
                var staffMember = db.StaffMember.SingleOrDefault(s => s.Id == staffId);
                requestedHoliday.Title = "Holiday";
                requestedHoliday.StaffMember = staffMember;
                requestedHoliday.IsApproved = false;
                requestedHoliday.End = requestedHoliday.End.AddDays(1); //end date is exclusive, but user will enter inclusive
                db.HolidayBooking.Add(requestedHoliday);
                await db.SaveChangesAsync();
            }

            return Ok();
        }
    }
}