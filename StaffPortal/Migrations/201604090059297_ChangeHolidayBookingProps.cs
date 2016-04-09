namespace StaffPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeHolidayBookingProps : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompanyHolidays", "Start", c => c.DateTime(nullable: false));
            AddColumn("dbo.CompanyHolidays", "End", c => c.DateTime(nullable: false));
            AddColumn("dbo.CompanyHolidays", "Title", c => c.String());
            AddColumn("dbo.HolidayBookings", "Start", c => c.DateTime(nullable: false));
            AddColumn("dbo.HolidayBookings", "End", c => c.DateTime(nullable: false));
            AddColumn("dbo.HolidayBookings", "Title", c => c.String());
            AddColumn("dbo.Sicknesses", "Start", c => c.DateTime(nullable: false));
            AddColumn("dbo.Sicknesses", "End", c => c.DateTime(nullable: false));
            AddColumn("dbo.Sicknesses", "Title", c => c.String());
            DropColumn("dbo.CompanyHolidays", "StartDate");
            DropColumn("dbo.CompanyHolidays", "EndDate");
            DropColumn("dbo.HolidayBookings", "StartDate");
            DropColumn("dbo.HolidayBookings", "EndDate");
            DropColumn("dbo.Sicknesses", "StartDate");
            DropColumn("dbo.Sicknesses", "EndDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sicknesses", "EndDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Sicknesses", "StartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.HolidayBookings", "EndDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.HolidayBookings", "StartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.CompanyHolidays", "EndDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.CompanyHolidays", "StartDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Sicknesses", "Title");
            DropColumn("dbo.Sicknesses", "End");
            DropColumn("dbo.Sicknesses", "Start");
            DropColumn("dbo.HolidayBookings", "Title");
            DropColumn("dbo.HolidayBookings", "End");
            DropColumn("dbo.HolidayBookings", "Start");
            DropColumn("dbo.CompanyHolidays", "Title");
            DropColumn("dbo.CompanyHolidays", "End");
            DropColumn("dbo.CompanyHolidays", "Start");
        }
    }
}
