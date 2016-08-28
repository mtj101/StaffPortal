namespace StaffPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveExplicitForeignKeyFromSickness : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Sicknesses", "StaffId", "dbo.StaffMembers");
            DropIndex("dbo.Sicknesses", new[] { "StaffId" });
            RenameColumn(table: "dbo.Sicknesses", name: "StaffId", newName: "User_Id");
            AlterColumn("dbo.Sicknesses", "User_Id", c => c.Int());
            CreateIndex("dbo.Sicknesses", "User_Id");
            AddForeignKey("dbo.Sicknesses", "User_Id", "dbo.StaffMembers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sicknesses", "User_Id", "dbo.StaffMembers");
            DropIndex("dbo.Sicknesses", new[] { "User_Id" });
            AlterColumn("dbo.Sicknesses", "User_Id", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Sicknesses", name: "User_Id", newName: "StaffId");
            CreateIndex("dbo.Sicknesses", "StaffId");
            AddForeignKey("dbo.Sicknesses", "StaffId", "dbo.StaffMembers", "Id", cascadeDelete: true);
        }
    }
}
