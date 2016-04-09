using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using StaffPortal.Models;

namespace StaffPortal.Controllers
{
    [Authorize]
    [RoutePrefix("calendar")]
    public class CalendarApiController : ApiController
    {
        [Route("bookeduserholidays")]
        [HttpGet]
        public IEnumerable<HolidayBooking> GetBookedHolidaysByStaffMember(DateTime start, DateTime end, int staffId)
        {
            var db = new ApplicationDbContext();
            var calEvts = db.HolidayBooking.Where(h => h.StaffMember.Id == staffId && h.IsApproved).ToList();
            return calEvts;
        }

        [Route("pendinguserholidays")]
        [HttpGet]
        public IEnumerable<HolidayBooking> GetPendingHolidaysByStaffMember(DateTime start, DateTime end, int staffId)
        {
            var db = new ApplicationDbContext();
            var calEvts = db.HolidayBooking.Where(h => h.StaffMember.Id == staffId && !h.IsApproved).ToList();
            return calEvts;
        }

        [AllowAnonymous]
        [Route("companyholidays")]
        [HttpGet]
        public IEnumerable<CompanyHoliday> GetCompanyHolidays(DateTime start, DateTime end)
        {
            var db = new ApplicationDbContext();
            var calEvts = new List<CompanyHoliday>{new CompanyHoliday()
            {
                Start = new DateTime(2016,4,10),
                End = new DateTime(2016,4,11),
                Title = "Easter"
            }
            };
            return calEvts;
        }

        [Route("{id}")]
        public string Get(int id)
        {
            return "value";
        }
    }
}