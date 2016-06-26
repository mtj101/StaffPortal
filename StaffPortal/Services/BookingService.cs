using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using StaffPortal.Models;

namespace StaffPortal.Services
{
    public class BookingService
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        public BookingService()
        {
        }

        public async Task<BookingResult> BookHoliday(int staffMemberId, DateTime start, DateTime end)
        {
            if (start >= end)
                return new BookingResult(isBooked: false, message: "Start date must be before end date");

            StaffMember staffMember = _db.StaffMember.SingleOrDefault(s => s.Id == staffMemberId);

            if (staffMember != null)
            {
                int departmentNum = _db.StaffMember.Single(s => s.Id == staffMemberId).DepartmentId;
                // all of the holidays of staff members in the same department as current staff member
                var bookingsForDepartment = _db.HolidayBooking.Where(hb => hb.StaffMember.DepartmentId == departmentNum).ToList();

                bool bookingIsValid = ValidateBooking(start, end, bookingsForDepartment);

                if (!bookingIsValid)
                {
                    return new BookingResult(isBooked: false,
                        message: "There is a clash with an existing holiday");
                }

                var holidayBooking = new HolidayBooking(staffMember, start, end);
                _db.HolidayBooking.Add(holidayBooking);
                await _db.SaveChangesAsync();

                return new BookingResult(isBooked: true, message: "Success");
            }

            // Staff member must have been invalid, return failure message
            return new BookingResult(isBooked: false, message: "No valid staff member found");
        }


        private bool ValidateBooking(DateTime start, DateTime end, IEnumerable<HolidayBooking> bookingsForDepartment)
        {
            var daysRequested = GetBusinessDays(start, end);

            var daysNotAvailable = new HashSet<DateTime>();
            foreach (var holidayBooking in bookingsForDepartment)
            {
                var days = GetBusinessDays(holidayBooking.Start, holidayBooking.End);
                foreach (var day in days)
                {
                    daysNotAvailable.Add(day);
                }
            }           
            bool clash = daysNotAvailable.Intersect(daysRequested).Any();

            return !clash;
        }


        public HolidayTotals GetHolidayTotalsForUser(int staffMember)
        {
            var db = new ApplicationDbContext();

            var yearStart = new DateTime(DateTime.Now.Year, 1, 1);
            var yearEnd = new DateTime(DateTime.Now.Year + 1, 1, 1);

            var totals = new HolidayTotals();

            var bookedHolidays =
                db.HolidayBooking
                    .Where(h => h.StaffMember.Id == staffMember && h.Start >= yearStart && h.End < yearEnd && h.IsApproved)
                    .AsEnumerable();

            totals.Booked = GetHolidayCountsForBookings(bookedHolidays);

            var pendingHolidays =
                db.HolidayBooking
                    .Where(
                        h => h.StaffMember.Id == staffMember && h.Start >= yearStart && h.End < yearEnd && !h.IsApproved)
                    .AsEnumerable();
            totals.Pending = GetHolidayCountsForBookings(pendingHolidays);

            return totals;
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

        private int GetNumberOfBusinessDays(DateTime startD, DateTime endD)
        {
            return GetBusinessDays(startD, endD).Count();
        }

        private int GetHolidayCountsForBookings(IEnumerable<HolidayBooking> holidayBookings)
        {
            var booked = holidayBookings.Aggregate(0, (total, hb) => total + GetNumberOfBusinessDays(hb.Start, hb.End));
            return booked;
        }
    }

    public static class DateTimeExtensions
    {
        public static bool IsWorkingDay(this DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
        }
    }
}