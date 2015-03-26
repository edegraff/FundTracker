namespace Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CapitalizedALetter : DbMigration
    {
        public override void Up()
        {
			//Since sql doesn't care about capitalization I don't think anything actually needs to be done
        }
        
        public override void Down()
        {
        }
    }
}
