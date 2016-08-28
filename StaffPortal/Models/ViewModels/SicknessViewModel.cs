using StaffPortal.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffPortal.Models.ViewModels
{
    public class SicknessViewModel
    {
        public Dictionary<StaffMember, List<Sickness>> StaffSicknesses { get; set; }
    }
}
