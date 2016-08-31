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
using StaffPortal.Business;
using StaffPortal.Models;
using StaffPortal.Service;


namespace StaffPortal.Controllers
{
    [Authorize]
    [RoutePrefix("alert")]
    public class AlertApiController : ApiController
    {
        private AlertService _alertService = new AlertService();

        [Route("{alertId:int}")]
        [HttpDelete]
        public IHttpActionResult RemoveHoliday(int alertId)
        {
            try
            {
                _alertService.RemoveAlert(alertId);
            }
            catch (ArgumentException e)
            {
                return BadRequest();
            }
            
            return Ok();
        }
    }
}