using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.Models;

namespace FundPortfolio.Tests
{
    [TestClass]
    public class NotificationsTest
    {
        [TestMethod]
        public void Notifications()
        {
            foreach(var n in TriggerList())
            {
                Assert.IsTrue(n.ShouldNotify());
            }

            foreach (var n in NoTriggerList())
            {
                Assert.IsFalse(n.ShouldNotify());
            }
        }

        // Returns a list of notifications that should trigger
        public List<Notification> TriggerList()
        {
            List<Notification> notifications = new List<Notification>();

            using (var db = new DatabaseContext())
            {
                var fund = db.Funds.Find("511");
                ChangeNotification cn = new ChangeNotification();
                cn.FundEntity = fund;
                cn.IsPercent = false;
                cn.ThresholdValue = 0.00000001f * (fund.CurrentValue - fund.GetValueByDate(DateTime.Now.Date - new TimeSpan(5,0,0,0,0)));
                cn.Days = -7;
                notifications.Add(cn);

                cn = new ChangeNotification();
                cn.FundEntity = fund;
                cn.IsPercent = true;
                cn.ThresholdValue = 0.00000001f * (fund.CurrentValue - fund.GetValueByDate(DateTime.Now.Date - new TimeSpan(5, 0, 0, 0, 0)));
                cn.Days = -7;
                notifications.Add(cn);

                ValueNotification vn = new ValueNotification();
                vn.FundEntity = fund;
                vn.IsAbove = true;
                vn.ThresholdValue = fund.CurrentValue - 1;
                notifications.Add(vn);

                vn = new ValueNotification();
                vn.FundEntity = fund;
                vn.IsAbove = false;
                vn.ThresholdValue = fund.CurrentValue + 1;
                notifications.Add(vn);

                return notifications;
            }
        }

        // Returns a list of notifications that should not trigger
        public List<Notification> NoTriggerList()
        {
            List<Notification> notifications = new List<Notification>();

            using (var db = new DatabaseContext())
            {
                var fund = db.Funds.Find("511");
                ChangeNotification cn = new ChangeNotification();
                cn.FundEntity = fund;
                cn.IsPercent = false;
                cn.ThresholdValue = 100;
                cn.Days = -7;
                notifications.Add(cn);

                cn = new ChangeNotification();
                cn.FundEntity = fund;
                cn.IsPercent = true;
                cn.ThresholdValue = 100;
                cn.Days = -7;
                notifications.Add(cn);

                ValueNotification vn = new ValueNotification();
                vn.FundEntity = fund;
                vn.IsAbove = true;
                vn.ThresholdValue = fund.CurrentValue + 1;
                vn.Days = -7;
                notifications.Add(vn);

                vn = new ValueNotification();
                vn.FundEntity = fund;
                vn.IsAbove = false;
                vn.ThresholdValue = fund.CurrentValue - 1;
                vn.Days = -7;
                notifications.Add(vn);

                return notifications;
            }
        }

    }
}
