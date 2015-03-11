using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
	public class FundProjector
	{
        public const int projectionLimit = 10;

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

        private void doProjection() {
            for (int i = 0; i < projectionLimit; i++)
            {
                // TODO Needs an actual prediction implementation
                this.Projection.Add(Fund.CurrentValue);
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
