using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
	[Table("ChangeNotification")]
	public class ChangeNotification : Notification
	{
        [Display(Name = "Percentage Threshold")]
		public bool IsPercent { get; set; }

		public override bool ShouldNotify()
		{
            List<float> deltas = new List<float>();
            if (this.Days <= 0)
            {
                for (int i = 1; i <= Math.Abs(this.Days); i++)
                {
                    deltas.Add(this.FundEntity.CurrentValue - this.FundEntity.GetValueByDate(DateTime.Now.Date - new TimeSpan(i, 0, 0, 0, 0)));
                }
            }
            else
            {
                // 3.2.3.3.3 Prediction values. Using the forecasting, users can be notified if a fund is likely to go down or up a certain amount.
                foreach (var v in new FundProjector(this.FundEntity, DateTime.Now.Date).Projection)
                {
                    deltas.Add(this.FundEntity.CurrentValue - v);
                }
                return false;
            }
            if(IsPercent)
            {
                // 3.2.3.3.2 Percentage change of fund value in a given time period.
                foreach(var d in deltas)
                {
                    float percent = d / this.FundEntity.CurrentValue * 100;
                    if(Math.Abs(percent) > Math.Abs(this.ThresholdValue) && (percent * this.ThresholdValue > 0))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                // 3.2.3.3.1 Change of fund value exceeding a set threshold over a specified period of time.
                foreach (var d in deltas)
                {
                    if(Math.Abs(d) > Math.Abs(this.ThresholdValue) && (d * this.ThresholdValue > 0))
                    {
                        return true;
                    }
                }
                return false;
            }
		}
	}
}
