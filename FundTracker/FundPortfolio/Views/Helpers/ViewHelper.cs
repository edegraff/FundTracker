using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FundPortfolio.Views.Helpers
{
	public static class InputExtensions
	{
		public static string AsPercent(this HtmlHelper helper, double? percent)
		{
			if (percent == null)
				return "-";
			return ((double)percent).ToString("P4");
		}
	}
}