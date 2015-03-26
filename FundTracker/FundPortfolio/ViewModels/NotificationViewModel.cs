using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FundPortfolio.ViewModels
{
	public class NotificationViewModel
	{
		public NotificationViewModel()
		{
			ValueNotification = new ValueNotification();
			ChangeNotification = new ChangeNotification();
		}
		public ValueNotification ValueNotification { get; set; }
		public ChangeNotification ChangeNotification { get; set; }
	}
}