namespace StaffPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StaffmemberStartDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StaffMembers", "StartDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StaffMembers", "StartDate");
        }
    }
}
