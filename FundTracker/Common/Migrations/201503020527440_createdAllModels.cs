namespace Common.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class createdAllModels : DbMigration
	{
		public override void Up()
		{
			CreateTable(
				"dbo.UserProfile",
				c => new
				{
					UserId = c.Int(nullable: false, identity: true),
					Email = c.String(maxLength: 128),
				})
				.PrimaryKey(t => t.UserId)
				.Index(t => t.UserId);

			CreateTable(
				"dbo.Notifications",
				c => new
					{
						NotificationId = c.Int(nullable: false, identity: true),
						UserId = c.Int(nullable: false),
						AutoReset = c.Boolean(nullable: false),
						ThresholdValue = c.Single(nullable: false),
						TimeSpan = c.Time(nullable: false, precision: 7),
						FundEntity_id = c.String(maxLength: 128),
					})
				.PrimaryKey(t => t.NotificationId)
				.ForeignKey("dbo.FundEntity", t => t.FundEntity_id)
				.ForeignKey("dbo.UserProfile", t => t.UserId, cascadeDelete: true)
				.Index(t => t.UserId)
				.Index(t => t.FundEntity_id);

			CreateTable(
				"dbo.UserTransaction",
				c => new
					{
						UserTransactionId = c.Int(nullable: false, identity: true),
						UserId = c.Int(nullable: false),
						FundEntityId = c.String(maxLength: 128),
						Date = c.DateTime(nullable: false),
						Value = c.Single(nullable: false),
					})
				.PrimaryKey(t => t.UserTransactionId)
				.ForeignKey("dbo.FundEntity", t => t.FundEntityId)
				.ForeignKey("dbo.UserProfile", t => t.UserId, cascadeDelete: true)
				.Index(t => t.UserId)
				.Index(t => t.FundEntityId);

			CreateTable(
				"dbo.ChangeNotification",
				c => new
					{
						NotificationId = c.Int(nullable: false),
						IsPercent = c.Boolean(nullable: false),
					})
				.PrimaryKey(t => t.NotificationId)
				.ForeignKey("dbo.Notifications", t => t.NotificationId)
				.Index(t => t.NotificationId);

			CreateTable(
				"dbo.ValueNotification",
				c => new
					{
						NotificationId = c.Int(nullable: false),
						IsAbove = c.Boolean(nullable: false),
					})
				.PrimaryKey(t => t.NotificationId)
				.ForeignKey("dbo.Notifications", t => t.NotificationId)
				.Index(t => t.NotificationId);
		}

		public override void Down()
		{
			AddColumn("dbo.UserProfile", "UserName", c => c.String());
			DropForeignKey("dbo.ValueNotification", "NotificationId", "dbo.Notifications");
			DropForeignKey("dbo.ChangeNotification", "NotificationId", "dbo.Notifications");
			DropForeignKey("dbo.UserTransaction", "UserId", "dbo.UserProfile");
			DropForeignKey("dbo.UserTransaction", "FundEntityId", "dbo.FundEntity");
			DropForeignKey("dbo.Notifications", "UserId", "dbo.UserProfile");
			DropForeignKey("dbo.Notifications", "FundEntity_id", "dbo.FundEntity");
			DropIndex("dbo.ValueNotification", new[] { "NotificationId" });
			DropIndex("dbo.ChangeNotification", new[] { "NotificationId" });
			DropIndex("dbo.UserTransaction", new[] { "FundEntityId" });
			DropIndex("dbo.UserTransaction", new[] { "UserId" });
			DropIndex("dbo.Notifications", new[] { "FundEntity_id" });
			DropIndex("dbo.Notifications", new[] { "UserId" });
			DropColumn("dbo.UserProfile", "Email");
			DropTable("dbo.ValueNotification");
			DropTable("dbo.ChangeNotification");
			DropTable("dbo.UserTransaction");
			DropTable("dbo.Notifications");
		}
	}
}
