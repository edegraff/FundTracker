namespace FundTracker.Common
{
	using System;
	using System.Data.Entity;
	using System.Linq;

	public class DatabaseContext : DbContext
	{
		public DatabaseContext()
			: base("AzureConnection")
		{
		
		}

		// Add a DbSet for each entity type that you want to include in your model. For more information 
		// on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

		public virtual DbSet<FundEntity> Funds { get; set; }
		public virtual DbSet<UserProfile> UserProfiles { get; set; }
	}


}