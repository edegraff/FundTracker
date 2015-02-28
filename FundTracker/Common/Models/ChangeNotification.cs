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
			return false;
		}
	}
}
