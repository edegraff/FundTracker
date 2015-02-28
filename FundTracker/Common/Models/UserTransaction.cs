﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
	[Table("UserTransaction")]
	public class UserTransaction
	{
		[Key]
		[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
		public int UserTransactionId { get; set; }

		public int UserId { get; set; }

		public String FundEntityId { get; set; }

		[ForeignKey("FundEntityId")]
		public FundEntity FundEntity { get; set; }

		[ForeignKey("UserId")]
		public UserProfile UserProfile { get; set;  }

		public DateTime Date { get; set; }
		public float Value { get; set; }
	}
}
