namespace StaffPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoreClasses3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sicknesses", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Sicknesses", "User_Id");
            AddForeignKey("dbo.Sicknesses", "User_Id", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.Sicknesses", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sicknesses", "UserId", c => c.String());
            DropForeignKey("dbo.Sicknesses", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Sicknesses", new[] { "User_Id" });
            DropColumn("dbo.Sicknesses", "User_Id");
        }
    }
}
