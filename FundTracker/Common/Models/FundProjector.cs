using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
	public class FundProjector
	{
        // How many days we will project for
        public const int projectionLimit = 3;

        public FundProjector(ITimeSeriesFundData fundentity, DateTime nowDate)
        {
            NowDate = nowDate.Date;
            Fund = fundentity;
            Projection = new List<float>();
            doProjection();
        }

        private DateTime NowDate { get; set;  }
		private ITimeSeriesFundData Fund { get; set; }
        public List<float> Projection { get; set;  }

        /*
         * Implements the Average(Average, Average Drift Combination) prediction.
         * 
         * First, claculate the Average Drift Combination prediction based on data from the last 6 months, (or less if data unavailable).
         * Take the average of the data (y1), pair with the midpoint (x1) of the data, and draw a line to the current value (x2, y2), extrapolate out one day.
         * 
         * Second, take the Average Drift Combination, and the average value for the last 6 months and average the two values for the result.
         */
        private void doProjection()
        {
            // Get the relevant historic data
            Tuple<List<float>, List<DateTime>> historic = this.Fund.GetDataInRange(this.NowDate.AddDays(-180), this.NowDate, this.NowDate);
            List<float> histVals = historic.Item1;
            List<DateTime> histDates = historic.Item2;
            removeNaNs(histVals, histDates);

            // Initialize some vars we will use
            float avg, count, avgDrift, result;
            float curVal = this.Fund.CurrentValue;

            // Calculate the projection for as many days as we can (somewhat reliably) project for, up to the projectionLimit
            for (int i = 0; i < projectionLimit; i++)
            {
                avg = histVals.Average();
                count = histDates.Count;

                avgDrift = (curVal - avg) / ((count - 1) / 2) + curVal;
                result = (avgDrift + avg) / 2;

                histVals.Add(result);
                histDates.Add(histDates.Last().AddDays(1));
                this.Projection.Add(result);

                curVal = result;
            }
        }

        private void removeNaNs(List<float> vals, List<DateTime> dates)
        {
            int i = 0;
            while (i < vals.Count)
            {
                if (float.IsNaN(vals[i]))
                {
                    vals.RemoveAt(i);
                    dates.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
	}
}
