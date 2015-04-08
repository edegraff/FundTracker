using Common.Models;
using FundPortfolio.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FundPortfolio.Tests.Controllers
{
	[TestClass]
	public class FundEntitiesControllerTest
	{
		private class FundEntityComparer : IEqualityComparer<FundEntity>
		{
			public bool Equals(FundEntity x, FundEntity y)
			{
				return x.Id == y.Id && x.Name != y.Name;
			}

			public int GetHashCode(FundEntity x)
			{
				return x.Id.GetHashCode();
			}
		}

		[TestMethod]
		public void Index()
		{
			using (var db = new DatabaseContext())
			{
				// Arrange
				var controller = new FundEntitiesController();
				var databaseFunds = db.Funds.ToList();

				// Act
				ViewResult result = controller.Index(null) as ViewResult;
				var allFunds = result.Model as IEnumerable<Common.Models.FundEntity>;

				// Assert
				Assert.IsNotNull(result);
				
				//Make sure all results that exist in the database exist
				Assert.Equals(databaseFunds.Count(), allFunds.Count());
				Assert.Equals(databaseFunds.Count(), allFunds.Intersect(databaseFunds, new FundEntityComparer()).Count());

			}
		}


		[TestMethod]
		public void Search()
		{
			// Arrange
			var controller = new FundEntitiesController();
			var searchTerm = "Canadian";

			// Act
			ViewResult searchResult = controller.Index(searchTerm) as ViewResult;
			ViewResult result = controller.Index(null) as ViewResult;
			var filteredFunds = searchResult.Model as IEnumerable<Common.Models.FundEntity>;
			var allFunds = result.Model as IEnumerable<Common.Models.FundEntity>;

			// Assert
			Assert.IsNotNull(filteredFunds);
			//Verify all results contain the search for term
			foreach (var fundEntity in filteredFunds)
			{
				Assert.IsTrue(fundEntity.Name.Contains(searchTerm));
			}

			//Verify we didn't miss any results
			foreach (var fundEntity in allFunds)
			{
				if (fundEntity.Name.Contains(searchTerm))
					Assert.IsTrue(filteredFunds.Contains(fundEntity, new FundEntityComparer()));
			}
		}

		//Most of details are a report. The only unique thing is the use of the method AverageOver so this is the only thing tested
		[TestMethod]
		public void Details()
		{
			var now = DateTime.Today;
			var fundEntity = new FundEntity();
			double expectedResult = 2.81f;
			double delta = 0.001f;

			fundEntity.FundHistory = new List<FundData>(){
				new FundData(){ Value = 100f, Date = now.AddYears(-3)},
				new FundData(){ Value = 115f, Date = now.AddYears(-2)},
				new FundData(){ Value = 103.50f, Date = now.AddYears(-1)},
				new FundData(){ Value = 108.67f, Date = now}
			};
			Assert.AreEqual( expectedResult, (double)fundEntity.AverageOver(DateTime.Now.AddYears(-3)), delta);

		}
	}
}
