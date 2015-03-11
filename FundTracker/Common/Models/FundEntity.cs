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
                {
                    throw new InvalidOperationException("There is no historic data for this fund");
                }
                else
                {
                    return FundHistory[FundHistory.Count - 1].Value;
                }
			}
			set
			{
				FundHistory.Add(new FundData() { Value = value, Date = DateTime.Now });
			}
		}

        public float GetValueByDate(DateTime date)
        {
            if (date.Date > DateTime.Now.Date)
            {
                throw new IndexOutOfRangeException("There is no data for this date yet.");
            }
            else
            {
                for (int i = FundHistory.Count - 1; i >= 0; i--)
                {
                    if (FundHistory[i].Date.Date <= date.Date) // Find the val of fund on the given day
                        return FundHistory[i].Value;
                }
                throw new InvalidOperationException("There is no historic data for this fund");
            }
        }
    }
}
