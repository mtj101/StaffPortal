using StaffPortal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace StaffPortal.Business
{
    public class HolidayManager
    {
        private readonly int _maxHolidaysPerDepartment;

        public HolidayManager(int maxHolidaysPerDepartment)
        {
            _maxHolidaysPerDepartment = maxHolidaysPerDepartment;
        }

        /// <summary>
        /// Returns a <see cref="BookingResult"/> indicating success of booking, and if successful will contain a <see cref="HolidayBooking"/> object.
        /// </summary>
        /// <param name="member">An existing staff member</param>
        /// <param name="start">Date of first holiday day</param>
        /// <param name="end">Date of return to work</param>
        /// <param name="unavailableDays">The Absences which contain the days that cannot contain ANY clashes (e.g. own holidays, company holidays)</param>
        /// <param name="departmentHolidays">The Absences which contain the days that could have clashes depending on business rules of concurrent holidays</param>
        /// <returns>BookingResult</returns>
        public BookingResult BookHoliday(StaffMember member, DateTime start, DateTime end,
            IEnumerable<Absence> unavailableDays, IEnumerable<Absence> departmentHolidays)
        {
            if (start >= end)
                return new BookingResult(null, "Start date must be before end date.", false);
            if (member == null)
                return new BookingResult(null, "A valid member must be provided.", false);

            bool bookingIsValid = ValidateBooking(start, end, unavailableDays, departmentHolidays);

            if (bookingIsValid)
            {
                return new BookingResult(new HolidayBooking(member, start, end), "Successful booking.", true);
            }
            return new BookingResult(null, "Unable to book, clash with current holiday.", false);
        }


        private bool ValidateBooking(DateTime start, DateTime end, IEnumerable<Absence> unavailableDays, IEnumerable<Absence> departmentHolidays)
        {
            var daysRequested = GetBusinessDays(start, end);

            // if duplicate days in unavailable days, just keep one
            var daysNotAvailable = new HashSet<DateTime>();
            if (unavailableDays != null)
            {
                foreach (var holiday in unavailableDays)
                {
                    var days = GetBusinessDays(holiday.Start, holiday.End);
                    foreach (var day in days)
                    {
                        daysNotAvailable.Add(day);
                    }
                }
            }
            bool clashUnavailable = daysNotAvailable.Intersect(daysRequested).Any();

            // if duplicate days in department holidays, keep both
            var departmentBookedDays = new List<DateTime>();
            if (departmentHolidays != null)
            {
                foreach (var holiday in departmentHolidays)
                {
                    var days = GetBusinessDays(holiday.Start, holiday.End);
                    foreach (var day in days)
                    {
                        departmentBookedDays.Add(day);
                    }
                }
            }
            bool clashDepartment = departmentBookedDays.GroupBy(d => d).Any(dt => daysRequested.Contains(dt.Key) && dt.Count() >= _maxHolidaysPerDepartment);


            return !(clashUnavailable || clashDepartment);
        }

        private IEnumerable<DateTime> GetBusinessDays(DateTime startD, DateTime endD)
        {
            var daysInRange = new List<DateTime>();
            // end of loop is "day < endD" rather than "day <= endD" since end date is exclusive
            for (DateTime day = startD; day < endD; day = day.AddDays(1))
            {
                if (day.IsWorkingDay())
                {
                    daysInRange.Add(day);
                }
            }
            return daysInRange;
        }

        /// <summary>
        /// Returns the number of days of which all the holiday bookings passed in span.
        /// </summary>
        /// <param name="holidayBookings">The holiday bookings to be counted</param>
        /// <returns>The number of days the bookings cover</returns>
        public int GetHolidayCountsForBookings(IEnumerable<HolidayBooking> holidayBookings)
        {
            var booked = holidayBookings.Aggregate(0, (total, hb) => total + GetNumberOfBusinessDays(hb.Start, hb.End));
            return booked;
        }

        public int GetNumberOfBusinessDays(DateTime startD, DateTime endD)
        {
            return GetBusinessDays(startD, endD).Count();
        }
    }
}