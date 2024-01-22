namespace CookHouse.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInfoToConfigSite : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ConfigSites", "EmailSend", c => c.String(maxLength: 100));
            AddColumn("dbo.ConfigSites", "EmailPass", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ConfigSites", "EmailPass");
            DropColumn("dbo.ConfigSites", "EmailSend");
        }
    }
}
