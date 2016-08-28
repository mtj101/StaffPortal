using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace StaffPortal.Business
{
    public class Sickness : Absence
    {
        public string Reason { get; set; }

        public StaffMember StaffMember { get; set; }
    }
}