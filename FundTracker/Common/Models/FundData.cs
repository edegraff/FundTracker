using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
	[Table("FundHistory")]
	public class FundData : IFundData
	{
		[Key, Column(Order = 0)]
		public String FundEntityId { get; set; }
		[Key, Column(Order = 1)]
		public DateTime Date { get; set; }

		public float Value { get; set; }

		[ForeignKey("FundEntityId")]
		public FundEntity FundEntity { get; set; }
	}

}
