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

        // GET: FundEntities
		public ActionResult Index(String searchTerm)
        {
			if (searchTerm == null)
				searchTerm = "";
			return View((from fund in db.Funds
						 where fund.Name.Contains(searchTerm)
						 select fund).ToList());
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
