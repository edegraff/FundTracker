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

        // GET: Report
        public ActionResult Index(String fundId)
        {
            return View();
        }

        // GET: CustomReport
        public ActionResult CustomReport(DateTime start, DateTime end, String fundIds, String graph)
        {
            @ViewBag.graph = graph;

            string[] ids = fundIds.Split(',');

            List<FundEntity> funds = new List<FundEntity>();

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
    }
}