namespace StaffPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStaffMemberToHolidayBooking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HolidayBookings", "StaffMember_Id", c => c.Int());
            CreateIndex("dbo.HolidayBookings", "StaffMember_Id");
            AddForeignKey("dbo.HolidayBookings", "StaffMember_Id", "dbo.StaffMembers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HolidayBookings", "StaffMember_Id", "dbo.StaffMembers");
            DropIndex("dbo.HolidayBookings", new[] { "StaffMember_Id" });
            DropColumn("dbo.HolidayBookings", "StaffMember_Id");
        }
    }
}
