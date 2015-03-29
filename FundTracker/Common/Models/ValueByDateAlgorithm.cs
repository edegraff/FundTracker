using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
	public static class TimeSeriesFundDataExtensions
	{
		/*
	 * Returns any historical data for the fund within the date range.  
	 * The list will be truncated for any day beyond nowDate.  (As there should not be data beyond 'now'.)
	 * Fills any unavailable data slot with float.NaN
	 * Ordered by earliest date first.
	 */
		public static Tuple<List<float>, List<DateTime>> GetDataInRange(this ITimeSeriesFundData timeSeriesData, DateTime start, DateTime end, DateTime nowDate)
		{
			if (nowDate == null)
				nowDate = DateTime.Now.Date;
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
			if (timeSeriesData.FundData.Count() == 0)
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
			IFundData previous = null;
			var arr = timeSeriesData.FundData.OrderBy(x => x.Date)
											.SkipWhile((fundData) => { if (fundData.Date.Date >= start) return false; previous = fundData; return true; })
											.TakeWhile(fundData => fundData.Date.Date <= end);

			var previousVal = float.NaN;
			if (previous != null)
				previousVal = previous.Value;
			var cur = arr.First();

			// Ensure we have early enough data for start date, otherwise fill with NaN
			while (cur.Date.Date >= start && start <= nowDate)
			{
				vals.Add(previousVal);
				dates.Add(start);
				start = start.AddDays(1);
			}


			return new Tuple<List<float>, List<DateTime>>(vals, dates);
		}
	}
}

