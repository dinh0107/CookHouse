namespace CookHouse.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class qrcode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ConfigSites", "QRImage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ConfigSites", "QRImage");
        }
    }
}
