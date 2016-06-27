using System;
using System.Collections.Generic;
using NUnit.Framework;
using StaffPortal.Business;

namespace StaffPortalTests.Business
{
    [TestFixture]
    public class HolidayManagerTests
    {
        private HolidayManager holidayManager;

        [SetUp]
        public void Setup()
        {
            holidayManager = new HolidayManager();
        }

        [Test]
        public void BookHolidayTest__Start_date_not_before_end_date_results_in_booking_failure()
        {
            // arrange
            var member = new StaffMember();
            var start = new DateTime(2016, 2, 2);
            var end = new DateTime(2016, 2, 2);

            // act
            var bookingResult = holidayManager.BookHoliday(member, start, end, null);

            // assert
            Assert.AreEqual("Start date must be before end date.", bookingResult.Message);
            Assert.False(bookingResult.IsBooked);
        }

        [Test]
        public void BookHolidayTest__null_member_results_in_booking_failure()
        {
            // arrange
            var start = new DateTime(2016, 2, 1);
            var end = new DateTime(2016, 2, 2);

            // act
            var bookingResult = holidayManager.BookHoliday(null, start, end, null);

            // assert
            Assert.AreEqual("A valid member must be provided.", bookingResult.Message);
            Assert.False(bookingResult.IsBooked);
        }

        [Test]
        public void BookHolidayTest__clashing_day_results_in_booking_failure()
        {
            // arrange
            var member = new StaffMember();
            var startNewBooking = new DateTime(2016, 2, 1);
            var endNewBooking = new DateTime(2016, 2, 2);
            var startExistingBooking = new DateTime(2016, 1, 29);
            var endExistingBooking = new DateTime(2016, 2, 2);
            var clash = new HolidayBooking(new StaffMember(), startExistingBooking, endExistingBooking);

            // act
            var bookingResult = holidayManager.BookHoliday(member, startNewBooking, endNewBooking, new List<Absence> {clash});

            // assert
            Assert.AreEqual("Unable to book, clash with current holiday.", bookingResult.Message);
            Assert.False(bookingResult.IsBooked);
        }

        [Test]
        public void BookHolidayTest__new_booking_ending_a_day_before_existing_results_in_booking_success()
        {
            // arrange
            var member = new StaffMember();
            var startNewBooking = new DateTime(2016, 2, 1);
            var endNewBooking = new DateTime(2016, 2, 2);
            var startExistingBooking = new DateTime(2016, 2, 2);
            var endExistingBooking = new DateTime(2016, 2, 3);
            var clash = new HolidayBooking(new StaffMember(), startExistingBooking, endExistingBooking);

            // act
            var bookingResult = holidayManager.BookHoliday(member, startNewBooking, endNewBooking, new List<Absence> { clash });

            // assert
            Assert.AreEqual("Successful booking.", bookingResult.Message);
            Assert.True(bookingResult.IsBooked);
        }

        [Test]
        public void BookHolidayTest__new_booking_starting_a_day_after_existing_results_in_booking_success()
        {
            // arrange
            var member = new StaffMember();
            var startNewBooking = new DateTime(2016, 2, 4);
            var endNewBooking = new DateTime(2016, 2, 8);
            var startExistingBooking = new DateTime(2016, 2, 2);
            var endExistingBooking = new DateTime(2016, 2, 4);
            var clash = new HolidayBooking(new StaffMember(), startExistingBooking, endExistingBooking);

            // act
            var bookingResult = holidayManager.BookHoliday(member, startNewBooking, endNewBooking, new List<Absence> { clash });

            // assert
            Assert.AreEqual("Successful booking.", bookingResult.Message);
            Assert.True(bookingResult.IsBooked);
        }

        [Test]
        public void BookHolidayTest__new_booking_starting_on_a_saturday_when_existing_ends_the_next_monday_is_successful()
        {
            // arrange
            var member = new StaffMember();
            var startNewBooking = new DateTime(2016, 6, 4);
            var endNewBooking = new DateTime(2016, 6, 10);
            var startExistingBooking = new DateTime(2016, 6, 2);
            var endExistingBooking = new DateTime(2016, 6, 6);
            var clash = new HolidayBooking(new StaffMember(), startExistingBooking, endExistingBooking);

            // act
            var bookingResult = holidayManager.BookHoliday(member, startNewBooking, endNewBooking, new List<Absence> { clash });

            // assert
            Assert.AreEqual("Successful booking.", bookingResult.Message);
            Assert.True(bookingResult.IsBooked);
        }

        [Test]
        public void GetHolidayCountsForBookingsTest__a_single_holiday_booking_during_weekday_returns_correct_number_of_days()
        {
            // arrange
            var start = new DateTime(2016, 6, 7);
            var end = new DateTime(2016, 6, 10);
            var booking = new HolidayBooking(new StaffMember(), start, end);

            // act
            var dayCount = holidayManager.GetHolidayCountsForBookings(new List<HolidayBooking> { booking });

            // assert
            Assert.AreEqual(3, dayCount);
        }

        [Test]
        public void GetHolidayCountsForBookingsTest__two_bookings_during_weekdays_returns_correct_total()
        {
            // arrange
            var start1 = new DateTime(2016, 6, 6);
            var end1 = new DateTime(2016, 6, 8);
            var booking1 = new HolidayBooking(new StaffMember(), start1, end1);

            var start2 = new DateTime(2016, 6, 9);
            var end2 = new DateTime(2016, 6, 10);
            var booking2 = new HolidayBooking(new StaffMember(), start2, end2);

            // act
            var dayCount = holidayManager.GetHolidayCountsForBookings(new List<HolidayBooking> { booking1, booking2 });

            // assert
            Assert.AreEqual(3, dayCount);
        }

        [Test]
        public void GetHolidayCountsForBookingsTest__a_single_booking_with_spanning_separate_weeks_returns_correct_total()
        {
            // arrange
            var start = new DateTime(2016, 6, 9);
            var end = new DateTime(2016, 6, 23);
            var booking = new HolidayBooking(new StaffMember(), start, end);

            // act
            var dayCount = holidayManager.GetHolidayCountsForBookings(new List<HolidayBooking> { booking });

            // assert
            Assert.AreEqual(10, dayCount);
        }

        [Test]
        public void GetHolidayCountsForBookingsTest__two_bookings_with_overlapping_weekends_returns_correct_total()
        {
            // arrange
            var start1 = new DateTime(2016, 6, 3);
            var end1 = new DateTime(2016, 6, 13);
            var booking1 = new HolidayBooking(new StaffMember(), start1, end1);

            var start2 = new DateTime(2016, 6, 11);
            var end2 = new DateTime(2016, 6, 20);
            var booking2 = new HolidayBooking(new StaffMember(), start2, end2);

            // act
            var dayCount = holidayManager.GetHolidayCountsForBookings(new List<HolidayBooking> { booking1, booking2 });

            // assert
            Assert.AreEqual(11, dayCount);
        }
    }
}