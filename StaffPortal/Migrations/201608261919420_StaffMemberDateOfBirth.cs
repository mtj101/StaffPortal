namespace StaffPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StaffMemberDateOfBirth : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StaffMembers", "DateOfBirth", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StaffMembers", "DateOfBirth");
        }
    }
}
