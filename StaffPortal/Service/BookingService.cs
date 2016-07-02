using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using StaffPortal.Business;
using StaffPortal.Models;

namespace StaffPortal.Service
{
    public class BookingService
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        private HolidayManager holidayManager = new HolidayManager();

        public BookingService()
        {
        }

        public async Task<BookingResult> BookHoliday(int staffId, DateTime start, DateTime end)
        {
            var staffMember = await _db.StaffMember.FindAsync(staffId);
            if (staffMember == null)
                return new BookingResult(null, $"No Staff Member with id {staffId} found", false);

            // all of the holidays of staff members in the same department as current staff member (but excluding the current staff member themselves)
            var bookingsForDepartment = await _db.HolidayBooking.Where(hb => hb.StaffMember.DepartmentId == staffMember.DepartmentId && hb.StaffMember.Id != staffMember.Id).ToListAsync();
            // the current member's existing holiday bookings
            var currentMembersHolidays = await _db.HolidayBooking.Where(hb => hb.StaffMember.Id == staffMember.Id).ToListAsync();
            // all the company holidays (i.e. cannot book a holiday on these days)
            var companyHolidays = await _db.CompanyHoliday.ToListAsync();

            List<Absence> unavailableDays = new List<Absence>();
            unavailableDays.AddRange(companyHolidays);
            unavailableDays.AddRange(currentMembersHolidays);

            var bookingResult = holidayManager.BookHoliday(staffMember, start, end, unavailableDays, bookingsForDepartment);

            if (bookingResult.IsBooked)
            {
                var currentHolidays = GetHolidayTotalsForUser(staffId);
                int maxDaysForMember = int.Parse(ConfigurationManager.AppSettings["maxHolidaysPerMember"]);
                bool areEnoughDaysInBalance = currentHolidays.Booked + currentHolidays.Pending +
                                              holidayManager.GetNumberOfBusinessDays(start, end) <= maxDaysForMember;

                if (areEnoughDaysInBalance)
                {
                    _db.HolidayBooking.Add(bookingResult.HolidayBooking);
                    await _db.SaveChangesAsync();
                    return bookingResult;
                }
                else
                {
                    return new BookingResult(null, "Maximum holidays have been exceeded", false);
                }

            }

            return bookingResult;
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

            totals.Booked = holidayManager.GetHolidayCountsForBookings(bookedHolidays);

            var pendingHolidays =
                db.HolidayBooking
                    .Where(
                        h => h.StaffMember.Id == staffMember && h.Start >= yearStart && h.End < yearEnd && !h.IsApproved)
                    .AsEnumerable();
            totals.Pending = holidayManager.GetHolidayCountsForBookings(pendingHolidays);

            return totals;
        }

    }
}