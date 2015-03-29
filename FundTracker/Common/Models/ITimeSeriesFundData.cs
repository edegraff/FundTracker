using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Models
{
	public interface ITimeSeriesFundData
	{
		string Name { get;  }

		float CurrentValue { get; }

		IEnumerable<IFundData> FundData { get; set; }
	}
}
