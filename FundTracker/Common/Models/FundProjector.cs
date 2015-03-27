using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
	public class FundProjector
	{
        public const int projectionLimit = 1;

        public FundProjector(FundEntity fundentity)
        {
            nowDate = DateTime.Now;
            Fund = fundentity;
            Projection = new List<float>();
            doProjection();
        }

        private DateTime nowDate { get; set;  }
        private FundEntity Fund { get; set; }
        private List<float> Projection { get; set;  }

        /*
         * Implements the Average(Average, Average Drift Combination) prediction.
         * 
         * First, claculate the Average Drift Combination prediction based on data from the last 6 months, (or less if data unavailable).
         * Take the average of the data (y1), pair with the midpoint (x1) of the data, and draw a line to the current value (x2, y2), extrapolate out one day.
         * 
         * Second, take the Average Drift Combination, and the average value for the last 6 months and average the two values for the result.
         */
        private void doProjection() {
            // Do we have 6 months of data?  If not, use what we have
            DateTime firstActualDayOfData = this.Fund.getFirstDate();
            DateTime date = DateTime.Now.AddDays(-180).Date;
            if (date < firstActualDayOfData)
            {
                date = firstActualDayOfData;
            }

            // Initialize some vars we will use
            Tuple<float, int> avginf = this.Fund.averageSince(date);
            float avg = avginf.Item1;
            float avgCount = avginf.Item2;
            float curVal = this.Fund.CurrentValue;
            float avgDrift, result;

            // Calculate the projection for as many days as we can (somewhat reliably) project for, up to the projectionLimit
            for (int i = 0; i < projectionLimit; i++)
            {
                avgDrift = (curVal - avg) / ( (avgCount-1) / 2)  +  curVal;
                result = (avgDrift + avg) / 2;

                this.Projection.Add(result);
            }
        }

        public float getPredictionByDate(DateTime reqDay) 
        {
            int dif = reqDay.Date.Subtract(nowDate.Date).Days;
            
            if (dif > projectionLimit)
            {
                throw new ArgumentOutOfRangeException("Requested date, " + reqDay.Date + ", is beyond the projection limit.");
            }
            else if (dif < 1)
            {
                throw new ArgumentOutOfRangeException("Requested date, " + reqDay.Date + ", is before the projection period.");
            }
            else
            {
                return this.Projection[dif - 1];
            }
        }
	}
}
