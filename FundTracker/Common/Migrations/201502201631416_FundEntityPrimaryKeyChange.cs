namespace Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FundEntityPrimaryKeyChange : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FundEntity",
                c => new
                    {
                        id = c.String(nullable: false, maxLength: 128),
                        currentDate = c.DateTime(nullable: false),
                        name = c.String(),
                        currentValue = c.Single(nullable: false),
                    })
                .PrimaryKey(t => new { t.id, t.currentDate });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FundEntity");
        }
    }
}
