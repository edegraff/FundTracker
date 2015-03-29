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
			var fundData = timeSeriesData.FundData.OrderBy(x => x.Date); // Sorts most current last
			IFundData cur = fundData.First();

			// Ensure we have early enough data for start date, otherwise fill with NaN
			while (cur.Date.Date >= start && start <= nowDate)
			{
				vals.Add(float.NaN);
				dates.Add(start);
				start = start.AddDays(1);
			}

			// Skip values that are before the requested period, but do not exceed timerSeriesData.FundData.Count
			int i = 0;
			while (i < fundData.Count() && fundData.ElementAt(i).Date.Date < start)
			{
				i++;
			}

			// Deal with the peroid between start and nowDate
			while (start <= nowDate && start <= end)
			{
				if (i < fundData.Count()) // Ensure i is in range
				{
					cur = fundData.ElementAt(i);
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
					vals.Add(fundData.ElementAt(i - 1).Value);
					dates.Add(start);
					start = start.AddDays(1);
				}
			}

			return new Tuple<List<float>, List<DateTime>>(vals, dates);
		}

	}
}

