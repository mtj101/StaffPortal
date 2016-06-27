namespace StaffPortal.Business
{
    public class BookingResult
    {
        public BookingResult(HolidayBooking holidayBooking, string message, bool isBooked)
        {
            HolidayBooking = holidayBooking;
            Message = message;
            IsBooked = isBooked;
        }

        public bool IsBooked { get; }

        public string Message { get; }

        public HolidayBooking HolidayBooking { get; }      
    }
}