using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using StaffPortal.Business;
using StaffPortal.Models;

namespace StaffPortal.Service
{
    public class SupervisorService
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public SupervisorService()
        {
        }

        public List<HolidayBooking> GetPendingHolidaysForSupervisor(int staffId)
        {
            var department = _db.StaffMember.Find(staffId).DepartmentId;
            var holidays = _db.HolidayBooking
                .Include(hb => hb.StaffMember)
                .Include(hb => hb.StaffMember.Department)
                .Where(hb => hb.StaffMember.DepartmentId == department)
                .Where(hb =>  hb.StaffMember.Id != staffId)
                .Where(hb => hb.IsApproved == false)
                .ToList();
            return holidays;
        }

        public List<HolidayBooking> GetAllPendingHolidays()
        {
            var holidays = _db.HolidayBooking
                .Include(hb => hb.StaffMember)
                .Include(hb => hb.StaffMember.Department)
                .Where(hb => hb.IsApproved == false)
                .ToList();
            return holidays;
        }
    }
}