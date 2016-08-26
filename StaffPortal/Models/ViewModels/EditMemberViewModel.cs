using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace StaffPortal.Models.ViewModels
{
    public class EditMemberViewModel
    {
        public int Id { get; set; }
        public IEnumerable<SelectListItem> Departments { get; set; }
        public string DepartmentName { get; set; }
        public IEnumerable<SelectListItem> Roles { get; set; }
        public string RoleName { get; set; }
        public string FirstNames { get; set; }
        public string Surname { get; set; }
        [RegularExpression(@"\d{9,}", ErrorMessage = "The PhoneNumber field contains an invalid number")]
        public string PhoneNumber { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
    }
}