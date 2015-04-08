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
	public class UserTransactionsControllerTest
	{
		[TestMethod]
		public void Index()
		{
			// Arrange
			var controller = new UserTransactionsController();

			// Act
			ViewResult result = controller.Index() as ViewResult;

			// Assert
			Assert.IsNotNull(result);
		}
	}
}
