namespace Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notificationsChage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "TimeSpan", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Notifications", "TimeSpan");
        }
    }
}
