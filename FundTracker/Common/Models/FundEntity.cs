using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Linq;

namespace Common.Models
{
	[Table("FundEntity")]
	public class FundEntity : ITimeSeriesFundData
	{
		public FundEntity()
		{
			FundHistory = new List<FundData>();
		}

		[Key]
		public String Id { get; set; }
		public string Name { get; set; }

		public virtual List<FundData> FundHistory { get; set; }

		public IEnumerable<IFundData> FundData
		{
			get
			{
				return FundHistory;
			}
			set
			{
				FundHistory = (List<FundData>)value;
			}
		}

		[NotMapped]
		public float CurrentValue
		{
			get
			{
				if (FundHistory.Count() == 0)
				{
					throw new InvalidOperationException("There is no historic data for this fund");
				}
				else
				{
					FundHistory.Sort((x, y) => x.Date.CompareTo(y.Date)); // Sorts most current last
					return FundData.Last().Value;
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
				FundHistory = FundHistory.OrderBy(x => x.Date).ToList(); // Sorts most current last
				for (int i = FundHistory.Count - 1; i >= 0; i--)
				{
					if (FundHistory[i].Date.Date <= date.Date) // Find the val of fund on the given day
						return FundHistory[i].Value;
				}
				throw new InvalidOperationException("There is no historic data for this fund");
			}
		}

		//Used this calculation http://www.investopedia.com/articles/08/annualized-returns.asp
		public double? AverageOver(DateTime minDate)
		{
			if (!FundHistory.Exists(f => f.Date <= minDate))
				return null;
			var fundsValues = this.GetDataInRange(minDate, DateTime.Now).Select( fd => fd.Value);
			if (fundsValues.Count() == 0)
				return null;

			return GeometicAverage(ToPercentChange(fundsValues));
		}

		private double GeometicAverage(IEnumerable<double> fundsValues)
		{
			double product = 1.0f;
			foreach (var value in fundsValues)
				product *= (1 + value);
			return (Math.Pow(product, (double)1 / fundsValues.Count()) - 1.0d);
		}

		private IEnumerable<double> ToPercentChange(IEnumerable<float> fundsValues)
		{
			var fundPercents = new List<double>();
			for (int i = 0; i < fundsValues.Count() - 1; i++)
				fundPercents.Add(CalculatePrecentChange(fundsValues.ElementAt(i), fundsValues.ElementAt(i + 1)));
			return fundPercents;
		}

		private double CalculatePrecentChange(float p1, float p2)
		{
			return (p2 - p1) / p1;
		}
	}
}
