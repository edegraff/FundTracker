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
    public class NotificationsControllerTest
    {
        [TestMethod]
        public void NotificationsController()
        {
            var userId = "39ee1136-9599-4f7c-8a4b-d5053bdb9690";//This refers to the user TestNotifications@fundtracker.com with password FundTracker1!
            DeleteNotifications(userId);
            // Preconditions
            Assert.IsTrue(GetNotifications(userId).Count == 0);
            
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

                var controller = new NotificationsController();
                controller.ControllerContext = controllerContext.Object;

                var n = new ChangeNotification();
                controller.Create(n, "511");
                Assert.IsTrue(GetNotifications(userId).Count > 0);

                controller.DeleteConfirmed(GetNotifications(userId).First().NotificationId);
                Assert.IsTrue(GetNotifications(userId).Count == 0);
            }
        }

        private List<Notification> GetNotifications(string userId)
        {
            using (var db = new DatabaseContext())
            {
                return db.Notifications.Where(n => n.UserId.Equals(userId)).ToList();
            }
        }

        private void DeleteNotifications(string userId)
        {
            using (var db = new DatabaseContext())
            {
               db.Notifications.RemoveRange(db.Notifications.Where(n => n.UserId.Equals(userId)));
               db.SaveChanges();
            }
        }
    }
}
