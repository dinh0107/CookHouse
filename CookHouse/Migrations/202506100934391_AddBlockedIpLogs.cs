namespace CookHouse.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBlockedIpLogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BlockedIpLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IP = c.String(maxLength: 50),
                        BlockedAt = c.DateTime(nullable: false),
                        RequestCount = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BlockedIpLogs");
        }
    }
}
