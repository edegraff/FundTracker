using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
	public class AggregateFundData : ITimeSeriesFundData
	{

		public float Units { get; set; }
		public string Name { get { return FundEntity.Name; } }

		public virtual List<FundData> AssestValues { get; set; }

		public IEnumerable<IFundData> FundData
		{
			get
			{
				return AssestValues;
			}
			set
			{
				AssestValues = (List<FundData>)value;
			}
		}

		public float CurrentValue
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

		public AggregateFundData(FundEntity fundEntity)
		{
			FundEntity = fundEntity;
			Units = 0;
		}

		public void CalculateValue(IEnumerable<IFundData> fundData)
		{
			Units = 0;
			foreach (var fundDatum in fundData)
				try
				{
					Units += fundDatum.Value / FundEntity.GetValueByDate(fundDatum.Date);
					AssestValues.Add(new FundData() { Value = CurrentValue });
				}
				catch (InvalidOperationException)
				{
					//We shouldn't allow it to even save invalid transactions but if they are what can we do
					//
				}

		}



	}
}
