using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
	public class AggregateFundValue
	{
		public float Units { get; set; }
		public String Name { get { return FundEntity.Name; } }
		public float Value
		{
			get
			{
				return FundEntity.CurrentValue * Units;
			}
			set
			{
				Units = value / FundEntity.CurrentValue;
			}
		}
		private FundEntity FundEntity { get; set; }

		public AggregateFundValue(FundEntity fundEntity)
		{
			FundEntity = fundEntity;
			Units = 0;
		}

		public void CalculateValue(IEnumerable<IFundData> fundData)
		{
			foreach (var fundDatum in fundData)
				try
				{
					Units += fundDatum.Value / FundEntity.GetValueByDate(fundDatum.Date);
				}
				catch (InvalidOperationException)
				{
					//We shouldn't allow it to even save invalid transactions but if they are what can we do
					//
				}

		}
	}
}
