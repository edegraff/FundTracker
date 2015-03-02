using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Common.Models;

namespace FundPortfolio.Controllers
{
	[Authorize]
    public class UserTransactionsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: UserTransactions
        public ActionResult Index()
        {
            var userTransactions = db.UserTransactions.Include(u => u.FundEntity).Include(u => u.UserProfile);
            return View(userTransactions.ToList());
        }

        // GET: UserTransactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTransaction userTransaction = db.UserTransactions.Find(id);
            if (userTransaction == null)
            {
                return HttpNotFound();
            }
            return View(userTransaction);
        }

        // GET: UserTransactions/Create
        public ActionResult Create()
        {
            ViewBag.FundEntityId = new SelectList(db.Funds, "id", "name");
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "Email");
            return View();
        }

        // POST: UserTransactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserTransactionId,UserId,FundEntityId,Date,Value")] UserTransaction userTransaction)
        {
            if (ModelState.IsValid)
            {
                db.UserTransactions.Add(userTransaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FundEntityId = new SelectList(db.Funds, "id", "name", userTransaction.FundEntityId);
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "Email", userTransaction.UserId);
            return View(userTransaction);
        }

        // GET: UserTransactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTransaction userTransaction = db.UserTransactions.Find(id);
            if (userTransaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.FundEntityId = new SelectList(db.Funds, "id", "name", userTransaction.FundEntityId);
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "Email", userTransaction.UserId);
            return View(userTransaction);
        }

        // POST: UserTransactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserTransactionId,UserId,FundEntityId,Date,Value")] UserTransaction userTransaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userTransaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FundEntityId = new SelectList(db.Funds, "id", "name", userTransaction.FundEntityId);
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "Email", userTransaction.UserId);
            return View(userTransaction);
        }

        // GET: UserTransactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserTransaction userTransaction = db.UserTransactions.Find(id);
            if (userTransaction == null)
            {
                return HttpNotFound();
            }
            return View(userTransaction);
        }

        // POST: UserTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserTransaction userTransaction = db.UserTransactions.Find(id);
            db.UserTransactions.Remove(userTransaction);
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
