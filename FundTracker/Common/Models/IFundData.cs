using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Models
{
	public interface IFundData
	{

		DateTime Date { get; set; }

		float Value { get; set; }

		FundEntity FundEntity { get; set; }
	}
}
