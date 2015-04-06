using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Common.Models;

namespace FundPortfolio.Controllers
{
    public class ReportController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        /*
         * 3.2.1.1.2 Visitors and Users will be able to compare mutual funds side by side. 
         * (This view allows users to select multiple funds to compare over a specified time period.)
         */
        // GET: CreateReport
        public ActionResult CreateReport()
        {
            return View((from fund in db.Funds
                         select fund).ToList());
        }

        /*
         * 3.2.1.1.1 Visitor and Users will be able to select fund from the search to see the history of the performance. 
         * (This view displays the history of performance for the fund.)
         *
         * 3.2.1.5.1 Predictions about the future performance/value of funds will be available, using existing prediction algorithms.  
         * (This view will return prediction values by default.)
         */
        // GET: Report
        public ActionResult Index(String id)
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
           
            var funds = new List<ITimeSeriesFundData>();
            funds.Add(fundEntity);

            DateTime start = DateTime.Now.AddMonths(-1);
            DateTime end = DateTime.Now.AddDays(FundProjector.projectionLimit);

            Report report = new Report(start, end, funds);
            return View(report);
        }

        /*
         * 3.2.1.1.2 Visitors and Users will be able to compare mutual funds side by side. 
         * (This view accepts multiple ids of funds that will be compared in the returned view with (optionally) a graph and tabular data)
         * 
         * 3.2.1.5.1 Predictions about the future performance/value of funds will be available, using existing prediction algorithms.  
         * (If users selected a date into the future when creating the report predictions will be shown.)
         */
        // GET: CustomReport
        public ActionResult CustomReport(DateTime start, DateTime end, String fundIds, bool graph)
        {
            ViewBag.Graph = graph;

            string[] ids = fundIds.Split(',');

            var funds = new List<ITimeSeriesFundData>();

            if (ids.Length == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                foreach (string id in ids)
                {
                    funds.Add(db.Funds.Find(id));
                }
            }

            // Ensure all requested funds were found
            for (int i = 0; i < funds.Count; i++)
            {
                if (funds[i] == null)
                {
                    return HttpNotFound();
                }
            }

            Report report = new Report(start, end, funds);
            return View(report);
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