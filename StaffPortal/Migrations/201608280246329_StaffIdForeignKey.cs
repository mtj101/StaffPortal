namespace StaffPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StaffIdForeignKey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Sicknesses", "User_Id", "dbo.StaffMembers");
            DropIndex("dbo.Sicknesses", new[] { "User_Id" });
            RenameColumn(table: "dbo.Sicknesses", name: "User_Id", newName: "StaffId");
            AlterColumn("dbo.Sicknesses", "StaffId", c => c.Int(nullable: false));
            CreateIndex("dbo.Sicknesses", "StaffId");
            AddForeignKey("dbo.Sicknesses", "StaffId", "dbo.StaffMembers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sicknesses", "StaffId", "dbo.StaffMembers");
            DropIndex("dbo.Sicknesses", new[] { "StaffId" });
            AlterColumn("dbo.Sicknesses", "StaffId", c => c.Int());
            RenameColumn(table: "dbo.Sicknesses", name: "StaffId", newName: "User_Id");
            CreateIndex("dbo.Sicknesses", "User_Id");
            AddForeignKey("dbo.Sicknesses", "User_Id", "dbo.StaffMembers", "Id");
        }
    }
}
