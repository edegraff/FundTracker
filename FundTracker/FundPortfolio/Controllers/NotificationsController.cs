using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Common.Models;
using FundPortfolio.ViewModels;
using System.Web.Security;
using FundPortfolio.Filters;
using Microsoft.AspNet.Identity;

namespace FundPortfolio.Controllers
{
    [Authorize, InitializeSimpleMembership]
    public class NotificationsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: Notifications
        public ActionResult Index()
        {
            var userId =  User.Identity.GetUserId();
            var notifications = db.Notifications.Include(n => n.UserProfile).Where(n => n.UserId.Equals(userId));
            return View(notifications.ToList());
        }

        public ActionResult IndexFunds()
        {
            return PartialView("_ListFundsPartial", db.Funds);
        }

        // GET: Notifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // GET: Notifications/Create
        public ActionResult Create()
        {
			ViewBag.types = new[] { typeof(ChangeNotification), typeof(ValueNotification) };
            return View(new NotificationViewModel());
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NotificationId,UserId,AutoReset,ThresholdValue,Days")] Notification notification, String fundId)
        {
            if (ModelState.IsValid)
            {
                notification.FundEntity = db.Funds.Find(fundId);
				notification.UserProfile = db.Users.Find(User.Identity.GetUserId());
                db.Notifications.Add(notification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "UserId", "Email", notification.UserId);
            return View(notification);
        }

        // GET: Notifications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Email", notification.UserId);

            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NotificationId,UserId,AutoReset,ThresholdValue,TimeSpan")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(notification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Email", notification.UserId);
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Notification notification = db.Notifications.Find(id);
            db.Notifications.Remove(notification);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Enable/Disables the notification
        public ActionResult Toggle(int? id)
        {
            Notification notification = db.Notifications.Find(id);
            notification.IsEnabled = !notification.IsEnabled;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
