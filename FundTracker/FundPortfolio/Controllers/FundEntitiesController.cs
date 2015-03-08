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
						 where fund.name.Contains(searchTerm)
						 select fund).ToList());
        }

        // GET: FundEntities/Details/5
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

        // GET: FundEntities/CreateReport
        public ActionResult CreateReport()
        {
            return View((from fund in db.Funds
                         select fund).ToList());
        } 

        // GET: FundEntities/Report
        public ActionResult Report(DateTime start, DateTime end, String fund0, String fund1, String fund2)
        {
            List<FundEntity> funds = new List<FundEntity>();

            if (fund0 == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                // Add non-null funds
                funds.Add(db.Funds.Find(fund0));

                if (fund1 != null)
                {
                    funds.Add(db.Funds.Find(fund1));

                    if (fund2 != null)
                    {
                        funds.Add(db.Funds.Find(fund2));
                    }
                }
            }
            
            // Ensure all requested funds were found
            for (int i=0; i < funds.Count; i++) {
                if (funds[i] == null)
                {
                    return HttpNotFound();
                }
            }

            return View(funds);
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
