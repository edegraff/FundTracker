namespace Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotificationsChange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "IsEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.Notifications", "Days", c => c.Int(nullable: false));
            DropColumn("dbo.Notifications", "TimeSpan");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Notifications", "TimeSpan", c => c.Time(nullable: false, precision: 7));
            DropColumn("dbo.Notifications", "Days");
            DropColumn("dbo.Notifications", "IsEnabled");
        }
    }
}
