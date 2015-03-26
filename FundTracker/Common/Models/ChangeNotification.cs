using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
	[Table("ChangeNotification")]
	public class ChangeNotification : Notification
	{
		public bool IsPercent { get; set; }

		public override bool ShouldNotify()
		{
            if(this.Days < 0)
            {
                List<float> deltas = new List<float>();
                for (int i = 1; i <= Math.Abs(this.Days); i++)
                {
                    deltas.Add(this.FundEntity.CurrentValue - this.FundEntity.GetValueByDate(DateTime.Now.Date - new TimeSpan(i,0,0,0,0)));
                }
                // Normal notification
                if(IsPercent)
                {
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
            else
            {
                // TODO: Implement after forecasting
                return false;
            }
		}
	}
}
