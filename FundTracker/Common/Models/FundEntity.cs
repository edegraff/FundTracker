using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;

namespace Common.Models
{
	[Table("FundEntity")]
    public class FundEntity
    {
		public FundEntity()
		{
			FundHistory = new List<FundData>();
		}

		[Key]
		public String id { get; set; }
		public string name { get; set; }

		public virtual List<FundData> FundHistory { get; set; }
		
		[NotMapped]
		public float CurrentValue
		{
			get
			{
				if (FundHistory.Count == 0)
					throw new InvalidOperationException("There is no historic data for this fund");
				return FundHistory[FundHistory.Count - 1].Value;
			}
			set
			{
				FundHistory.Add(new FundData() { Value = value, Date = DateTime.Now });
			}
		}
    }
}
