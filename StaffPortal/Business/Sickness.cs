using System;
using System.Threading.Tasks;

namespace StaffPortal.Business
{
    public class Sickness : Absence
    {
        public string Reason { get; set; }

        public StaffMember User { get; set; }
    }

    public static class DateTimeExtensions
    {
        public static bool IsWorkingDay(this DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
        }
    }
}