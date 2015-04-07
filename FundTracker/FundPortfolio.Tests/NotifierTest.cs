using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.Models;

namespace FundPortfolio.Tests
{
    [TestClass]
    public class NotifierTest
    {
        [TestMethod]
        public void TestNotifications()
        {
            //DeleteNotifications();
            //PopulateNotifications();
            ReadNotifications();
            FundService.Notifier notifier = new FundService.Notifier();
            //notifier.Notify();
        }

        public void PopulateNotifications()
        {
            using (var db = new DatabaseContext())
            {
                ChangeNotification n = new ChangeNotification();
                n.UserId = Guid.NewGuid().ToString();
                n.FundEntity = db.Funds.Find("511");
                Console.WriteLine(n.FundEntity.Name);
                n.IsPercent = false;
                n.ThresholdValue = 0.003f;
                n.Days = -7;
                //n.IsAbove = true;
                n.UserProfile = new UserProfile();

                db.Notifications.Add(n);

                db.SaveChanges();
            }
        }

        public void DeleteNotifications()
        {
            using (var db = new DatabaseContext())
            {
                List<Notification> list = new List<Notification>();
                foreach(var n in db.Notifications)
                {
                    list.Add(n);
                }
                db.Notifications.RemoveRange(list);
                db.SaveChanges();
            }
        }

        public void ReadNotifications()
        {
            using (var db = new DatabaseContext())
            {
                foreach (var n in db.Notifications)
                {
                    Console.WriteLine(n.FundEntity.Name);
                }
            }
        }
    }
}
