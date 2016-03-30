namespace StaffPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HolidayUser : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.HolidayBookings", name: "UserId", newName: "User_Id");
            RenameIndex(table: "dbo.HolidayBookings", name: "IX_UserId", newName: "IX_User_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.HolidayBookings", name: "IX_User_Id", newName: "IX_UserId");
            RenameColumn(table: "dbo.HolidayBookings", name: "User_Id", newName: "UserId");
        }
    }
}
