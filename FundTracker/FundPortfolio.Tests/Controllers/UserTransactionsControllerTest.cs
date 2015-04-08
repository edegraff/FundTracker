using Common.Models;
using FundPortfolio.Controllers;
using FundPortfolio.ViewModels;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FundPortfolio.Tests.Controllers
{
	[TestClass]
	public class UserTransactionsControllerTest
	{
		[TestMethod]
		public void UserTransactionsIndex()
		{
			// Arrange
			var userId = "535e3c53-21f8-431e-8bb9-369144c6a198";//This refers to the user TestUserTransactions@23Asd.com with password @23Asd
			var delta = 0.004;
			using (var context = ShimsContext.Create())
			{

				Microsoft.AspNet.Identity.Fakes
					.ShimIdentityExtensions.GetUserIdIIdentity =
				(i) => userId;

				var fakeHttpContext = new Mock<HttpContextBase>();
				var fakeIdentity = new GenericIdentity("User");
				var principal = new GenericPrincipal(fakeIdentity, null);

				fakeHttpContext.Setup(t => t.User).Returns(principal);

				var controllerContext = new Mock<ControllerContext>();
				controllerContext.Setup(t => t.HttpContext)
					.Returns(fakeHttpContext.Object);

				var controller = new UserTransactionsController();
				controller.ControllerContext = controllerContext.Object;

				// Act
				ViewResult result = controller.Index() as ViewResult;
				UserTransactionIndexViewModel model = result.Model as UserTransactionIndexViewModel;

				// Assert
				Assert.IsNotNull(model.GraphReport);
				Assert.AreEqual(100, model.TotalPaid);
				Assert.AreEqual(133.33, model.TotalAssets, delta);
				Assert.AreEqual(0.33, model.TotalNetPercentProft, delta);
				// Make sure all transactions belong to the specific person
				foreach (var trans in model.UserTransactions)
					Assert.AreEqual(userId, trans.UserId);

			}

		}

		[TestMethod]
		public void UserTransactionsCrud()
		{
			var userId = "535e3c53-21f8-431e-8bb9-369144c6a198";//This refers to the user TestUserTransactions@23Asd.com with password @23Asd
			var fundId = 300.ToString();

			var userTrans = new UserTransaction
			{
				UserId = userId,
				Date = DateTime.Now,
				Value = 100,
				FundEntityId = fundId.ToString()
			};
			using (var context = ShimsContext.Create())
			{

				Microsoft.AspNet.Identity.Fakes
					.ShimIdentityExtensions.GetUserIdIIdentity =
				(i) => userId;

				var fakeHttpContext = new Mock<HttpContextBase>();
				var fakeIdentity = new GenericIdentity("User");
				var principal = new GenericPrincipal(fakeIdentity, null);

				fakeHttpContext.Setup(t => t.User).Returns(principal);

				var controllerContext = new Mock<ControllerContext>();
				controllerContext.Setup(t => t.HttpContext)
					.Returns(fakeHttpContext.Object);

				var controller = new UserTransactionsController();
				controller.ControllerContext = controllerContext.Object;

				int? transId = null;
				using (var db = new DatabaseContext())
				{
					Assert.IsNull(db.UserTransactions.SingleOrDefault(ut => ut.FundEntityId == fundId && ut.UserId == userId  && ut.Value == 100));
					
					controller.Create(userTrans);
					Assert.IsNotNull(db.UserTransactions.Single(ut => ut.FundEntityId == fundId && ut.UserId == userId && ut.Value == 100));
					
					userTrans.Value = 200;
					controller.Edit(userTrans);
					Assert.IsNotNull(db.UserTransactions.Single(ut => ut.FundEntityId == fundId && ut.UserId == userId  && ut.Value == 200));

					transId = db.UserTransactions.Single(ut => ut.FundEntityId == fundId && ut.UserId == userId && ut.Value == 200).UserTransactionId;
				}
				
				//Must erase EF's cache, this seems a bug from EF may exist as I would think the desired behaviour is that when something is deleted even if it is
				//cached the cached one will be deleted.

				using (var db = new DatabaseContext())
				{

					controller.DeleteConfirmed(userTrans.UserTransactionId);
					Assert.IsNull(db.UserTransactions.SingleOrDefault(ut => ut.FundEntityId == fundId && ut.UserId == userId && ut.Value == 200));

					Assert.IsNull(db.UserTransactions.Find(transId));
				}
			

			}
		}
	}
}
