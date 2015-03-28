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
    public class FundEntity
    {
		public FundEntity()
		{
			FundHistory = new List<FundData>();
		}

		[Key]
		public String Id { get; set; }
		public string Name { get; set; }

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
                    FundHistory.Sort((x, y) => x.Date.CompareTo(y.Date)); // Sorts most current last
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
                FundHistory.OrderBy(x => x.Date); // Sorts most current last
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
			var fundsValues = (from data in FundHistory
						where data.Date >= minDate
						orderby data.Date
						select data.Value).ToList();
			if (fundsValues.Count == 0)
				return null;

			return GeometicAverage(ToPercentChange(fundsValues));
		}

		private double GeometicAverage(List<double> fundsValues)
		{
			double product = 1.0f;
			foreach (var value in fundsValues)
				product *= (1 + value);
			return (Math.Pow(product, (double) 1 / fundsValues.Count()) - 1.0d);
		}

		private List<double> ToPercentChange(List<float> fundsValues)
		{
			var fundPercents = new List<double>();
			for( int i = 0; i < fundsValues.Count() - 1; i++ )
				fundPercents.Add(CalculatePrecentChange(fundsValues.ElementAt(i), fundsValues.ElementAt(i + 1)));
			return fundPercents;
		}

		private double CalculatePrecentChange(float p1, float p2)
		{
			return (p2 - p1) / p1;
		}

        /*
         * Returns any historical data for the fund within the date range.  
         * The list will be truncated for any day beyond nowDate.  (As there should not be data beyond 'now'.)
         * Fills any unavailable data slot with float.NaN
         * Ordered by earliest date first.
         */
        public Tuple<List<float>, List<DateTime>> getDataInRange(DateTime start, DateTime end, DateTime nowDate)
        {
            if (nowDate == null)
            {
                nowDate = DateTime.Now.Date;
            }

            start = start.Date;
            end = end.Date;
            nowDate = nowDate.Date;
            
            if (start > end)
            {
                throw new ArgumentException("Start date after End date.");
            }

            List<float> vals = new List<float>();
            List<DateTime> dates = new List<DateTime>();

            // If no data return vals filled with NaN
            if (FundHistory.Count == 0)
            {
                while (start <= end)
                {
                    vals.Add(float.NaN);
                    dates.Add(start);
                    start = start.AddDays(1);
                }
                return new Tuple<List<float>, List<DateTime>>(vals, dates);
            }

            // Else, assemble the data
            FundHistory.OrderBy(x => x.Date); // Sorts most current last
            FundData cur = FundHistory.First();

            // Ensure we have early enough data for start date, otherwise fill with NaN
            while (cur.Date.Date >= start && start <= nowDate) 
            {
                vals.Add(float.NaN);
                dates.Add(start);
                start = start.AddDays(1);
            }

            // Skip values that are before the requested period, but do not exceed FundHistory.Count
            int i = 0;
            while (i < FundHistory.Count && FundHistory[i].Date.Date < start) 
            {
                i++;
            }

            // Deal with the peroid between start and nowDate
            while (start <= nowDate && start <= end)
            {
                if (i < FundHistory.Count) // Ensure i is in range
                {
                    cur = FundHistory[i];
                }

                if (cur.Date.Date == start) // found exact date match
                {
                    vals.Add(cur.Value);
                    dates.Add(start);
                    i++;
                    start = start.AddDays(1);
                }
                else // no specific data recorded for this day
                {
                    vals.Add(FundHistory[i - 1].Value);
                    dates.Add(start);
                    start = start.AddDays(1);
                }
            }

            return new Tuple<List<float>, List<DateTime>>(vals, dates);
        }
	}
}
