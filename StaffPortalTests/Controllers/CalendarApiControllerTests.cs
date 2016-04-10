using Microsoft.VisualStudio.TestTools.UnitTesting;
using StaffPortal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace StaffPortal.Controllers.Tests
{
    [TestClass]
    public class CalendarApiControllerTests
    {
        [TestMethod]
        public async Task BookHoliday_returns_400_if_start_date_is_after_end_date()
        {
            var controller = new CalendarApiController();
            var result =
                await controller.BookHoliday(new Models.HolidayBooking
                {
                    Start = new DateTime(2016, 1, 2),
                    End = new DateTime(2016, 1, 1)
                });

            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));      
        }

        [TestMethod]
        public async Task BookHoliday_returns_error_message_string_if_start_date_is_after_end_date()
        {
            var controller = new CalendarApiController();
            var result =
                await controller.BookHoliday(new Models.HolidayBooking
                {
                    Start = new DateTime(2016, 1, 2),
                    End = new DateTime(2016, 1, 1)
                });

            Assert.AreEqual("Start date must be before end date.", ((BadRequestErrorMessageResult)result).Message);
        }
    }
}