using System;

namespace StaffPortal.Business
{
    public class HolidayBooking : Absence
    {
        public bool IsApproved { get; set; }

        public StaffMember StaffMember { get; set; }

        public HolidayBooking() { }

        public HolidayBooking(StaffMember staffMember, DateTime start, DateTime end)
        {
            if (start >= end)
                throw new ArgumentException("Start date must be before end date");

            this.StaffMember = staffMember;
            this.Start = start;
            this.End = end;
            this.Title = "Holiday";
        }
    }
}