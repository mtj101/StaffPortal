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
        [Route("")]
        public IEnumerable<HolidayBooking> Get(DateTime start, DateTime end, int staffId)
        {

            var db = new ApplicationDbContext();
            var calEvts = db.HolidayBooking.Where(h => h.StaffMember.Id == staffId).ToList();
            return calEvts;
        }

        [Route("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}