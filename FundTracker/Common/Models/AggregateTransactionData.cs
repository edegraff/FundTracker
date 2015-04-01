using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
	public class AggregateTransactionData : ITimeSeriesFundData
	{


		public float Units { get; set; }

		[Display(Name = "Total Cost of Shares Bought ($)")]
		public float TotalPaid { get; set; }

		[Display(Name = "Net Profit (%)")]
		public float NetPercentProfit { get { return (CurrentValue - TotalPaid) / TotalPaid; } }

		public string Name { get { return FundEntity.Name; } }

		public virtual List<FundData> AssestValues { get; set; }

		public IEnumerable<IFundData> FundData
		{
			get
			{
				return AssestValues;
			}
		}

		[Display(Name = "Value ($)")]
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

		public AggregateTransactionData(Models.FundEntity fundEntity, IEnumerable<IFundData> transactionList)
		{
			AssestValues = new List<FundData>();
			FundEntity = fundEntity;
			Units = 0;
			TotalPaid = 0;
			Calculate(CalculateTotalPaidAndUnits(transactionList));
		}

		public void Calculate(IEnumerable<IFundData> unitList)
		{
			unitList = unitList.OrderBy(tl => tl.Date);
			var fundValueList = FundEntity.GetDataInRange(unitList.First().Date, DateTime.Today);

			float currentUnits = 0;
			float currentValue = 0;
			DateTime currentDate = DateTime.MinValue;

			while (fundValueList.Count() != 0 || unitList.Count() != 0)
			{
				if (fundValueList.Count() != 0 && unitList.Count() != 0 && fundValueList.First().Date.Date == unitList.First().Date.Date)
				{
					currentDate = fundValueList.First().Date.Date;
					currentValue = fundValueList.First().Value;
					fundValueList = fundValueList.Skip(1);
					currentUnits = unitList.First().Value;
					unitList = unitList.Skip(1);
				}
				else if (fundValueList.Count() != 0 && (unitList.Count() == 0 || (fundValueList.First().Date < unitList.First().Date)))
				{
					currentDate = fundValueList.First().Date.Date;
					currentValue = fundValueList.First().Value;
					fundValueList = fundValueList.Skip(1);
				}
				else if (unitList.Count() != 0)
				{
					currentDate = unitList.First().Date.Date;
					currentUnits = unitList.First().Value;
					unitList = unitList.Skip(1);
				}
				if (currentUnits != 0)
					AssestValues.Add(new FundData() { Value = currentUnits * currentValue, Date = currentDate });

			}
		}

		private IEnumerable<IFundData> CalculateTotalPaidAndUnits(IEnumerable<IFundData> transactionData)
		{
			var unitList = new List<FundData>();
			foreach (var transaction in transactionData)
			{
				var currentFundValue = FundEntity.GetValueByDate(transaction.Date);
				Units += transaction.Value / currentFundValue;
				TotalPaid += transaction.Value;
				unitList.Add(new FundData() { Value = Units, Date = transaction.Date });
			}
			return unitList;
		}



	}
}
