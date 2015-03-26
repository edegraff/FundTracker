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
        public Notification()
        {
            this.IsEnabled = true;
            this.AutoReset = false;
        }

		[Key]
		[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
		public int NotificationId { get; set; }

		public int UserId { get; set; }

		public bool AutoReset { get; set; }
        public bool IsEnabled { get; set; }
		public virtual FundEntity FundEntity { get; set; }
		public float ThresholdValue { get; set; }
		public int Days { get; set; }

		[ForeignKey("UserId")]
		public virtual UserProfile UserProfile { get; set; }

		public abstract bool ShouldNotify();
	}



}
