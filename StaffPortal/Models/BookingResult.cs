namespace StaffPortal.Models
{
    public class BookingResult
    {
        public bool IsBooked { get; }

        public string Message { get; }

        public BookingResult(bool isBooked, string message)
        {
            IsBooked = isBooked;
            Message = message;
        }

        public override bool Equals(object obj)
        {
            var obj2 = obj as BookingResult;
            if (obj2 != null)
            {
                return IsBooked == obj2.IsBooked && Message == obj2.Message;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Message.Length;
        }
    }
}