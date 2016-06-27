using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using StaffPortal.Business;

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
}