using System.ComponentModel.DataAnnotations.Schema;

namespace StaffPortal.Business
{
    public class StaffMember
    {
        public int Id { get; set; }
        public string Surname { get; set; }
        public string FirstNames { get; set; }


        public int DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; }
    }
}