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
    public class FundEntitiesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

		// 3.2.1.1.1 Visitor and Users will be able to select fund from the search to see the history of the performance.
		// 3.2.1.2.1 Visitors and Users will be able to search for mutual funds by name
		public ActionResult Index(String searchTerm)
        {
			if (searchTerm == null)
				searchTerm = "";
			return View((from fund in db.Funds
						 where fund.Name.Contains(searchTerm)
						 select fund).ToList());
        }

		// 3.2.1.1.1 Visitor and Users will be able to select fund from the search to see the history of the performance.
		public ActionResult Details(string id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			
			FundEntity fundEntity = db.Funds.Find(id);
			if (fundEntity == null)
			{
				return HttpNotFound();
			}
			return View(fundEntity);
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
