using NUnit;
using StaffPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using StaffPortal.Business;

namespace StaffPortal.Models.Tests
{
    [TestFixture]
    public class HolidayBookingTests
    {
        private StaffMember staffMember;

        [SetUp]
        public void Setup()
        {
            staffMember = new StaffMember();
        }

        [Test]
        public void HolidayBookingTest_NewInstanceCreatedIfStartBeforeEnd()
        {
            var start = new DateTime(2016, 1, 1);
            var end = new DateTime(2016, 1, 2);

            var holidayBooking = new HolidayBooking(staffMember, start, end);

            Assert.IsInstanceOf<HolidayBooking>(holidayBooking);
        }


        [Test]
        public void HolidayBookingTest_StartAfterEndThrowsException()
        {
            var start = new DateTime(2016, 1, 2);
            var end = new DateTime(2016, 1, 1);         

            Assert.Throws<ArgumentException>(() => new HolidayBooking(staffMember, start, end));
        }

        [Test]
        public void HolidayBookingTest_StartSameAsEndThrowsException()
        {
            var start = new DateTime(2016, 1, 1);
            var end = new DateTime(2016, 1, 1);

            Assert.Throws<ArgumentException>(() => new HolidayBooking(staffMember, start, end));
        }

        [Test]
        public void HolidayBookingTest_NewBookingInitialisesAsUnapproved()
        {
            var start = new DateTime(2016, 1, 1);
            var end = new DateTime(2016, 1, 2);

            var booking = new HolidayBooking(staffMember, start, end);

            Assert.IsFalse(booking.IsApproved);
        }
    }
}