namespace StaffPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StaffMemberAddressAndPhone : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StaffMembers", "PhoneNumber", c => c.String());
            AddColumn("dbo.StaffMembers", "Address1", c => c.String());
            AddColumn("dbo.StaffMembers", "Address2", c => c.String());
            AddColumn("dbo.StaffMembers", "City", c => c.String());
            AddColumn("dbo.StaffMembers", "County", c => c.String());
            AddColumn("dbo.StaffMembers", "PostCode", c => c.String());
            AddColumn("dbo.StaffMembers", "Country", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StaffMembers", "Country");
            DropColumn("dbo.StaffMembers", "PostCode");
            DropColumn("dbo.StaffMembers", "County");
            DropColumn("dbo.StaffMembers", "City");
            DropColumn("dbo.StaffMembers", "Address2");
            DropColumn("dbo.StaffMembers", "Address1");
            DropColumn("dbo.StaffMembers", "PhoneNumber");
        }
    }
}
