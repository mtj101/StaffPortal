namespace StaffPortal.Models
{ 
    public class LoggedInStaffMember
    {
        public int Id { get; set; }
        public int HolidaysBooked { get; set; }
        public int HolidaysPending { get; set; }
        public int MaximumHolidays { get; set; }
        public int Sickness { get; set; }
    }
}