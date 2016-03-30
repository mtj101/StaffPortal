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

        public DbSet<HolidayBooking> HolidayBooking { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<CompanyHoliday> CompanyHoliday { get; set; }
        public DbSet<Sickness> Sickness { get; set; }
        public DbSet<Alert> Alert { get; set; }
    }

    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

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
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class HolidayBooking : Absence
    {
        public bool IsApproved { get; set; }

        public ApplicationUser User { get; set; }
    }

    public class CompanyHoliday : Absence
    {
        public CompanyHolidayType CompanyHolidayType { get; set; }
    }

    public class Sickness : Absence
    {
        public string Reason { get; set; }

        public ApplicationUser User { get; set; }
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