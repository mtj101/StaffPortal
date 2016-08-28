using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace StaffPortal.Business
{
    public class Sickness : Absence
    {
        public string Reason { get; set; }

        public int StaffId { get; set; }

        [ForeignKey(nameof(StaffId))]
        public StaffMember User { get; set; }
    }
}