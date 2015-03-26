using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Common.Models;
using System.Web.Security;
using System.ComponentModel.DataAnnotations;
using PagedList;

namespace FundPortfolio.Controllers
{
	[Authorize]
    public class UserTransactionsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: UserTransactions
        public ActionResult Index()
        {
			var currentUserId = (int) Membership.GetUser().ProviderUserKey;
			var userTransactions = db.UserTransactions.OrderByDescending(ut => ut.Date).Include(u => u.FundEntity).Include(u => u.UserProfile).Where(u => u.UserId == currentUserId);
			var fundListViewModel = new FundListsViewModel() { 
				AggregateFunds = new List<AggregateFundValue>(), 
				UserTransactions = userTransactions.ToList()
			};
			
			foreach( var transactionList in userTransactions.GroupBy(ut => ut.FundEntityId ).ToList() )
			{
				var aggregateFundValue = new AggregateFundValue(transactionList.First().FundEntity);
				aggregateFundValue.CalculateValue(transactionList);
				fundListViewModel.TotalAssets += aggregateFundValue.Value;
				fundListViewModel.AggregateFunds.Add(aggregateFundValue);
			}

			
            return View(fundListViewModel);
        }

        // GET: UserTransactions/Create
        public ActionResult Create()
        {
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
				userTransaction.UserProfile = db.UserProfiles.Find(Membership.GetUser().ProviderUserKey);
                db.UserTransactions.Add(userTransaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

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

	public class FundListsViewModel
	{
		public List<AggregateFundValue> AggregateFunds { get; set; }
		public List<UserTransaction> UserTransactions { get; set; }

		[Display(Name = "Total Assets")]
		public float TotalAssets { get; set; }
	}
}
