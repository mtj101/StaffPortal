namespace StaffPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoreClasses : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.HolidayBookings", "IsApproved", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "Department_Id", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "Department_Id");
            AddForeignKey("dbo.AspNetUsers", "Department_Id", "dbo.Departments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "Department_Id", "dbo.Departments");
            DropIndex("dbo.AspNetUsers", new[] { "Department_Id" });
            DropColumn("dbo.AspNetUsers", "Department_Id");
            DropColumn("dbo.HolidayBookings", "IsApproved");
            DropTable("dbo.Departments");
        }
    }
}
