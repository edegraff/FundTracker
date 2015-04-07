namespace Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConvertToMVC5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Notifications", "UserId", "dbo.UserProfile");
			DropForeignKey("dbo.UserTransaction", "UserId", "dbo.UserProfile");
			DropForeignKey("dbo.Notifications", "UserId", "dbo.AspNetUsers");
			DropForeignKey("dbo.UserTransaction", "UserId", "dbo.AspNetUsers");
			DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
			DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
			DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
			DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Notifications", new[] { "UserId" });
            DropIndex("dbo.UserTransaction", new[] { "UserId" });
            DropPrimaryKey("dbo.AspNetUsers");
            
           
            AlterColumn("dbo.Notifications", "UserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.AspNetUsers", "Email", c => c.String(maxLength: 256));
            AlterColumn("dbo.UserTransaction", "UserId", c => c.String(maxLength: 128));

            AddPrimaryKey("dbo.AspNetUsers", "Id");
            CreateIndex("dbo.Notifications", "UserId");
            CreateIndex("dbo.UserTransaction", "UserId");
            //AddForeignKey("dbo.Notifications", "UserId", "dbo.AspNetUsers", "Id");
			//AddForeignKey("dbo.UserTransaction", "UserId", "dbo.AspNetUsers", "Id");
			AddForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles", "Id");
			AddForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers", "Id");
			AddForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers", "Id");
			AddForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers", "Id");
        }

        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "UserId", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.UserTransaction", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Notifications", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.UserTransaction", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Notifications", new[] { "UserId" });
            DropPrimaryKey("dbo.AspNetUsers");
            AlterColumn("dbo.UserTransaction", "UserId", c => c.Int(nullable: false));
            AlterColumn("dbo.AspNetUsers", "Email", c => c.String());
            AlterColumn("dbo.Notifications", "UserId", c => c.Int(nullable: false));
            DropColumn("dbo.AspNetUsers", "UserName");
            DropColumn("dbo.AspNetUsers", "AccessFailedCount");
            DropColumn("dbo.AspNetUsers", "LockoutEnabled");
            DropColumn("dbo.AspNetUsers", "LockoutEndDateUtc");
            DropColumn("dbo.AspNetUsers", "TwoFactorEnabled");
            DropColumn("dbo.AspNetUsers", "PhoneNumberConfirmed");
            DropColumn("dbo.AspNetUsers", "PhoneNumber");
            DropColumn("dbo.AspNetUsers", "SecurityStamp");
            DropColumn("dbo.AspNetUsers", "PasswordHash");
            DropColumn("dbo.AspNetUsers", "EmailConfirmed");
            DropColumn("dbo.AspNetUsers", "Id");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            AddPrimaryKey("dbo.AspNetUsers", "UserId");
            CreateIndex("dbo.UserTransaction", "UserId");
            CreateIndex("dbo.Notifications", "UserId");
            AddForeignKey("dbo.UserTransaction", "UserId", "dbo.UserProfile", "UserId", cascadeDelete: true);
            AddForeignKey("dbo.Notifications", "UserId", "dbo.UserProfile", "UserId", cascadeDelete: true);
            RenameTable(name: "dbo.AspNetUsers", newName: "UserProfile");
        }
    }
}
