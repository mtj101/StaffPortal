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

        public List<StaffMember> GetStaffForSupervisor(int supervisorStaffId, bool isAdmin)
        {
            var supervisor = _db.Users
                .Include(u => u.StaffMember)
                .SingleOrDefault(u => u.StaffMemberId == supervisorStaffId);


            var staff = new List<StaffMember>();
            if (isAdmin) // admin can see all users
            {
                staff = _db.StaffMember.ToList();
            }
            else
            {
                staff = _db.Users
                .Include(u => u.StaffMember)
                .Where(u => u.StaffMember.DepartmentId == supervisor.StaffMember.DepartmentId) // supervisors only see staff in same department
                .Where(u => u.Roles.FirstOrDefault().RoleId == "3") // only get regular users, not other supervisors in same department
                .Select(u => u.StaffMember)
                .ToList();
            }

            return staff;
        }

        public List<Sickness> GetSicknessesForStaff(IEnumerable<StaffMember> staff)
        {
            var staffIds = staff.Select(s => s.Id).ToArray();

            var sickness = _db.Sickness
                .Include(s => s.User)
                .Where(s => staffIds.Contains(s.User.Id))
                .ToList();

            return sickness;
        }

        public void AddSickness(Sickness sickness)
        {
            _db.StaffMember.Attach(sickness.User);
            _db.Sickness.Add(sickness);
            _db.SaveChanges();
        }
    }
}