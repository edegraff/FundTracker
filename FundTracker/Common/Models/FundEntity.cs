using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;

namespace FundTracker.Common
{
	[Table("FundEntity")]
    public class FundEntity
    {
		[Key, Column(Order = 0)]
		public String id { get; set; }


		public string name { get; set; }
		public float currentValue { get; set; }
		[Key, Column(Order = 1)]
		public DateTime currentDate { get; set; }
    }
}
