namespace StaffPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddApplicationSettingTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationSettings",
                c => new
                    {
                        SettingName = c.String(nullable: false, maxLength: 128),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.SettingName);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ApplicationSettings");
        }
    }
}
