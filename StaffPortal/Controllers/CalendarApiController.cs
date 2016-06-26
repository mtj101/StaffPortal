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
using StaffPortal.Services;


namespace StaffPortal.Controllers
{
    [Authorize]
    [RoutePrefix("calendar")]
    public class CalendarApiController : ApiController
    {
        private ApplicationUserManager _userManager;
        private BookingService _bookingService => new BookingService();

        public CalendarApiController()
        {
        }

        public CalendarApiController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
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

        [Route("holidayinsamedepartment")]
        [HttpGet]
        public IEnumerable<HolidayBooking> GetHolidaysForSameDepartment(DateTime start, DateTime end, int staffId)
        {
            var db = new ApplicationDbContext();
            int department = db.StaffMember.Single(s => s.Id == staffId).DepartmentId;
            var calEvts = db.HolidayBooking.Where(h => h.StaffMember.DepartmentId == department && h.StaffMember.Id != staffId && h.Start >= start && h.End <= end).ToList();
            foreach (var booking in calEvts)
            {
                booking.Title = "Unavailable";
            }
            return calEvts;
        }

        [AllowAnonymous]
        [Route("companyholidays")]
        [HttpGet]
        public IEnumerable<CompanyHoliday> GetCompanyHolidays(DateTime start, DateTime end)
        {
            var db = new ApplicationDbContext();
            var calEvts = db.CompanyHoliday.Where(ch => ch.Start >= start && ch.End <= end).ToList();
            return calEvts;
        }

        [Route("bookholiday")]
        [HttpPost]
        public async Task<IHttpActionResult> BookHoliday(BookingDateRange dates)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid dates entered.");

            int staffId = (await UserManager.FindByNameAsync(User.Identity.Name)).StaffMemberId;

            var bookingResult = await _bookingService.BookHoliday(staffId, dates.Start, dates.End);

            if (bookingResult.IsBooked)
            {
                return Ok();
            }

            // If we got this far, there must be something wrong
            return BadRequest(bookingResult.Message);
        }

        [Route("holidaytotals")]
        [HttpGet]
        public IHttpActionResult GetHolidayTotalsForUser(int staffId)
        {
            var holidayCounts = _bookingService.GetHolidayTotalsForUser(staffId);
            return Ok(holidayCounts);
        }
    }
}