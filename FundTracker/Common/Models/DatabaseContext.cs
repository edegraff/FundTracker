
namespace Common.Models
{
	using Microsoft.AspNet.Identity.EntityFramework;
	using System;
	using System.Data.Entity;
	using System.Linq;

	public class DatabaseContext : IdentityDbContext<UserProfile>
	{
		public DatabaseContext()
			: base("AzureConnection")
		{

		}

		// Add a DbSet for each entity type that you want to include in your model. For more information 
		// on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

		public virtual DbSet<FundEntity> Funds { get; set; }

		public System.Data.Entity.DbSet<Common.Models.Notification> Notifications { get; set; }

		public System.Data.Entity.DbSet<Common.Models.UserTransaction> UserTransactions { get; set; }

		public System.Data.Entity.DbSet<Common.Models.FundData> FundDatas { get; set; }

		public static DatabaseContext Create()
		{
			return new DatabaseContext();
		}
	}


}