namespace StaffPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HolidayBookingForeignKey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HolidayBookings", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.HolidayBookings", "UserId");
            AddForeignKey("dbo.HolidayBookings", "UserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HolidayBookings", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.HolidayBookings", new[] { "UserId" });
            DropColumn("dbo.HolidayBookings", "UserId");
        }
    }
}
