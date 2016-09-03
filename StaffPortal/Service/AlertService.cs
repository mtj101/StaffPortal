using StaffPortal.Business;
using StaffPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace StaffPortal.Service
{
    public class AlertService
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public void SendAlert(ApplicationUser user, string message)
        {
            var alert = CreateAlert(user, message);
            _db.Alert.Add(alert);

            _db.SaveChanges();
        }

        public void SendAlert(IEnumerable<ApplicationUser> users, string message)
        {
            foreach (var user in users)
            {
                var alert = CreateAlert(user, message);
                _db.Alert.Add(alert);
            }
            _db.SaveChanges();
        }

        private Alert CreateAlert(ApplicationUser user, string message)
        {
            _db.Users.Attach(user);

            var alert = new Alert
            {
                Created = DateTime.UtcNow,
                IsRead = false,
                Message = message,
                ForUser = user
            };

            return alert;
        }

        public void SendHolidayRequestAlert(int staffMemberId)
        {
            var staffMember = _db.StaffMember.Include(s => s.Department).FirstOrDefault(s => s.Id == staffMemberId);

            var user = _db.Users.Include(u => u.Roles).FirstOrDefault(u => u.StaffMemberId == staffMemberId);

            var usersToSendAlertTo = new List<ApplicationUser>();

            if (user.Roles.FirstOrDefault().RoleId == "3") // regular user, send to supervisor
            {
                usersToSendAlertTo = GetSupervisorsForDepartment(staffMember.Department);
            }
            else // supervisors/admins, send to admin
            {
                usersToSendAlertTo = _db.Users.Where(u => u.Roles.FirstOrDefault().RoleId == "1").ToList();
            }

            string message = $"{staffMember.FirstNames} {staffMember.Surname} has booked a holiday. Check \"Pending Requests\" to take action.";
            SendAlert(usersToSendAlertTo, message);
        }

        public void SendHolidayApprovalAlert(int[] holidayIds)
        {
            var holidayBookings = _db.HolidayBooking.Include(h => h.StaffMember).Where(h => holidayIds.Contains(h.Id)).ToList();

            foreach (var holidayBooking in holidayBookings)
            {
                var user = _db.Users.Single(u => u.StaffMemberId == holidayBooking.StaffMember.Id);
                string approvalMessage = $"Your holiday booked for {holidayBooking.Start.ToShortDateString()} - {holidayBooking.End.ToShortDateString()} has been approved";
                SendAlert(user, approvalMessage);
            }
        }

        internal void sendHolidayDenialAlert(int[] deniedHolidays)
        {
            var holidayBookings = _db.HolidayBooking.Include(h => h.StaffMember).Where(h => deniedHolidays.Contains(h.Id)).ToList();

            foreach (var holidayBooking in holidayBookings)
            {
                var user = _db.Users.Single(u => u.StaffMemberId == holidayBooking.StaffMember.Id);
                string denialMessage = $"Your holiday booked for {holidayBooking.Start.ToShortDateString()} - {holidayBooking.End.ToShortDateString()} was denied";
                SendAlert(user, denialMessage);
            }
        }

        private List<ApplicationUser> GetSupervisorsForDepartment(Department department)
        {
            var supervisors = _db.Users
                .Where(u => u.StaffMember.Department.Id == department.Id)
                .Where(u => u.Roles.FirstOrDefault().RoleId == "2")
                .ToList();

            return supervisors;
        }

        public List<Alert> GetAlertsForUser(string userId)
        {
            var alerts = _db.Alert.Where(a => a.ForUser.Id == userId).Where(a => a.IsRead == false).ToList();
            return alerts;
        }

        public void RemoveAlert(int alertId)
        {
            var alert = _db.Alert.SingleOrDefault(a => a.Id == alertId);

            if (alert == null)
                throw new ArgumentException();

            _db.Alert.Remove(alert);
            _db.SaveChanges();
        }

    }
}