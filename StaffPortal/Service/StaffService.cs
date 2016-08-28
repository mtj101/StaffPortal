using StaffPortal.Business;
using StaffPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaffPortal.Service
{
    public class StaffService
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public StaffMember GetStaffMemberById(int staffId)
        {
            var staffMember = _db.StaffMember.Single(s => s.Id == staffId);
            return staffMember;
        }
    }
}