﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Common.Models
{
	public abstract class Notification
	{

		[Key]
		[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
		public int NotificationId { get; set; }

		public int UserId { get; set; }

		public bool AutoReset { get; set; }
        public bool IsEnabled { get; set; }
		public FundEntity FundEntity { get; set; }
		public float ThresholdValue { get; set; }
		[Display(Name="Days before or after the current date to notify")]
		public int Days { get; set; }

		[ForeignKey("UserId")]
		public UserProfile UserProfile { get; set; }

		public abstract bool ShouldNotify();
	}



}
