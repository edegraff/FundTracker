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
