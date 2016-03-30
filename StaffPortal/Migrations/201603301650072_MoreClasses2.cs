namespace StaffPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoreClasses2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Alerts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Created = c.DateTime(nullable: false),
                        Message = c.String(),
                        IsRead = c.Boolean(nullable: false),
                        ForUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ForUser_Id)
                .Index(t => t.ForUser_Id);
            
            CreateTable(
                "dbo.CompanyHolidays",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyHolidayType = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sicknesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        Reason = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Alerts", "ForUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Alerts", new[] { "ForUser_Id" });
            DropTable("dbo.Sicknesses");
            DropTable("dbo.CompanyHolidays");
            DropTable("dbo.Alerts");
        }
    }
}
