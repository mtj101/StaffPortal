using System;
using System.ComponentModel.DataAnnotations;

namespace StaffPortal.Controllers
{
    public class AddSicknessViewModel
    {
        [DataType(DataType.Date)]
        public DateTime Start { get; set; }

        [DataType(DataType.Date)]
        public DateTime End { get; set; }

        public string Reason { get; set; }

        public int StaffId { get; set; }
    }
}