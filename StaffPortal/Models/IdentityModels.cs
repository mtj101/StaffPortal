using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace StaffPortal.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public virtual DbSet<HolidayBooking> HolidayBooking { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<CompanyHoliday> CompanyHoliday { get; set; }
        public virtual DbSet<Sickness> Sickness { get; set; }
        public virtual DbSet<Alert> Alert { get; set; }
        public virtual DbSet<StaffMember> StaffMember { get; set; }
    }

    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }

        public int StaffMemberId { get; set; }

        [ForeignKey("StaffMemberId")]
        public StaffMember StaffMember { get; set; }
    }

    public class StaffMember
    {
        public int Id { get; set; }
        public string Surname { get; set; }
        public string FirstNames { get; set; }


        public int DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; }
    }

    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public abstract class Absence
    {
        public int Id { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Title { get; set; }
    }

    public class HolidayBooking : Absence
    {
        public bool IsApproved { get; set; }

        public StaffMember StaffMember { get; set; }

        private HolidayBooking() { }

        public HolidayBooking(StaffMember staffMember, DateTime start, DateTime end)
        {
            if (start >= end)
                throw new ArgumentException("Start date must be before end date");

            this.StaffMember = staffMember;
            this.Start = start;
            this.End = end;
            this.Title = "Holiday";
        }
    }

    public class CompanyHoliday : Absence
    {
        public CompanyHolidayType CompanyHolidayType { get; set; }
    }

    public class Sickness : Absence
    {
        public string Reason { get; set; }

        public StaffMember User { get; set; }
    }

    public enum CompanyHolidayType
    {
        BankHoliday,
        TrainingDay
    }

    public class Alert
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public ApplicationUser ForUser { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
    }
}