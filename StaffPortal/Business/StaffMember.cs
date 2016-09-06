using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaffPortal.Business
{
    public class StaffMember
    {
        public int Id { get; set; }
        public string Surname { get; set; }
        public string FirstNames { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }

        public DateTime StartDate { get; set; }


        public int DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; }
    }
}