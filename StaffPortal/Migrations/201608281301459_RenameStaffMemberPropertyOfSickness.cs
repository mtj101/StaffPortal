namespace StaffPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameStaffMemberPropertyOfSickness : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Sicknesses", name: "User_Id", newName: "StaffMember_Id");
            RenameIndex(table: "dbo.Sicknesses", name: "IX_User_Id", newName: "IX_StaffMember_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Sicknesses", name: "IX_StaffMember_Id", newName: "IX_User_Id");
            RenameColumn(table: "dbo.Sicknesses", name: "StaffMember_Id", newName: "User_Id");
        }
    }
}
