using System.Collections.Generic;
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
    }
}