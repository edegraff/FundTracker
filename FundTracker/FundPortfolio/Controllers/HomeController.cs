using FundTracker.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FundPortfolio.Controllers
{
	[RequireHttps]
	public class HomeController : Controller
	{
		Database db = new Database();

		public ActionResult Index()
		{
			ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your app description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}

		public ActionResult SearchResults(String searchBox)
		{
			ViewBag.Funds = (from funds in db.Funds
							 where funds.name.Contains(searchBox)
							 group funds by funds.id into eachFund
							 select eachFund.FirstOrDefault()).ToList();
			var k = (from funds in db.Funds select funds).ToList();
			return View();
		}
	}
}
