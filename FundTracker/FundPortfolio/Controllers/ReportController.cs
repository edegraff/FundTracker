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

        // GET: CreateReport
        public ActionResult CreateReport()
        {
            return View((from fund in db.Funds
                         select fund).ToList());
        } 

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

        // GET: CustomReport
        public ActionResult CustomReport(DateTime start, DateTime end, String fundIds, bool graph)
        {
            ViewBag.Graph = graph;

            //3.2.1.1.2 Visitors and Users will be able to compare mutual funds side by side. (The ids of the funds that will be compared)
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