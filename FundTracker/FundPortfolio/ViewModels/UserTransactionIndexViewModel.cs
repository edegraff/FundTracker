using Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FundPortfolio.ViewModels
{
	public class UserTransactionIndexViewModel
	{
		public List<AggregateFundData> AggregateFunds { get; set; }
		public List<UserTransaction> UserTransactions { get; set; }

		public Report GraphReport { get; set; }
		[Display(Name = "Total Assets")]
		public float TotalAssets { get; set; }
	}
}