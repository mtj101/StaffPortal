using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StaffPortal.Models.ViewModels
{
    public class UserPanelViewModel
    {
        public string Name { get; set; }
        [Display(Name = "Date of Birth")]
        public string DateOfBirth { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        [Display(Name = "Start Date")]
        public string StartDate { get; set; }
        public string Department { get; set; }
        public string Role { get; set; }
        [Display(Name = "Employment Length")]
        public string EmploymentLength { get; set; }
        public string Email { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

    }
}