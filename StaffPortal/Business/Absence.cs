using System;

namespace StaffPortal.Business
{
    public abstract class Absence
    {
        public int Id { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Title { get; set; }
    }
}