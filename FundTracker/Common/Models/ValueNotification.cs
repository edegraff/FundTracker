using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    [Table("ValueNotification")]
    public class ValueNotification : Notification
    {
        public bool IsAbove { get; set; }

        public override bool ShouldNotify()
        {
            using (var db = new DatabaseContext())
            {
                if (IsAbove)
                {
                    return this.FundEntity.CurrentValue > this.ThresholdValue;
                }
                else
                {
                    return this.FundEntity.CurrentValue < this.ThresholdValue;
                }
            }
        }
    }
}
