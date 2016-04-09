namespace StaffPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForeignKeyForDepartmentToStaff : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StaffMembers", "Department_Id", "dbo.Departments");
            DropIndex("dbo.StaffMembers", new[] { "Department_Id" });
            RenameColumn(table: "dbo.StaffMembers", name: "Department_Id", newName: "DepartmentId");
            AlterColumn("dbo.StaffMembers", "DepartmentId", c => c.Int(nullable: false));
            CreateIndex("dbo.StaffMembers", "DepartmentId");
            AddForeignKey("dbo.StaffMembers", "DepartmentId", "dbo.Departments", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StaffMembers", "DepartmentId", "dbo.Departments");
            DropIndex("dbo.StaffMembers", new[] { "DepartmentId" });
            AlterColumn("dbo.StaffMembers", "DepartmentId", c => c.Int());
            RenameColumn(table: "dbo.StaffMembers", name: "DepartmentId", newName: "Department_Id");
            CreateIndex("dbo.StaffMembers", "Department_Id");
            AddForeignKey("dbo.StaffMembers", "Department_Id", "dbo.Departments", "Id");
        }
    }
}
