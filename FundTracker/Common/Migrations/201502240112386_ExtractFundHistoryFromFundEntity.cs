namespace Common.Migrations
{
	using Common.Models;
	using System;
	using System.Data.Entity.Migrations;
	using System.Linq;

	public partial class ExtractFundHistoryFromFundEntity : DbMigration
	{
		public override void Up()
		{
			DropPrimaryKey("dbo.FundEntity");
			CreateTable(
				"dbo.FundHistory",
				c => new
					{
						FundEntityId = c.String(nullable: false, maxLength: 128),
						Date = c.DateTime(nullable: false),
						Value = c.Single(nullable: false),
					})
				.PrimaryKey(t => new { t.FundEntityId, t.Date })
				.ForeignKey("dbo.FundEntity", t => t.FundEntityId, cascadeDelete: true);

			Sql("insert into dbo.fundhistory (FundEntityId, Date, Value) SELECT Id, currentdate, currentvalue FROM fundentity");

			DropColumn("dbo.FundEntity", "currentDate");
			DropColumn("dbo.FundEntity", "currentValue");

			Sql(@"	WITH cte AS (
					  SELECT[Name],row_number() 
					  OVER(PARTITION BY Name ORDER BY Id) AS [rn]
					  FROM dbo.FundEntity
					)
					DELETE cte WHERE [rn] > 1");
			AddPrimaryKey("dbo.FundEntity", "Id");
		}

		public override void Down()
		{
			AddColumn("dbo.FundEntity", "currentValue", c => c.Single(nullable: false));
			AddColumn("dbo.FundEntity", "currentDate", c => c.DateTime(nullable: false));
			DropForeignKey("dbo.FundHistory", "FundEntityId", "dbo.FundEntity");
			DropIndex("dbo.FundHistory", new[] { "FundEntityId" });
			DropPrimaryKey("dbo.FundEntity");

			Sql("");

			DropTable("dbo.FundHistory");
			AddPrimaryKey("dbo.FundEntity", new[] { "Id", "currentDate" });
		}
	}
}
