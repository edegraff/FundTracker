using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
		}

		[Display(Name="Value")]
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
			AssestValues = new List<FundData>();
			FundEntity = fundEntity;
			Units = 0;
		}

		public void CalculateValue(IEnumerable<IFundData> transactionData)
		{
			Units = 0;
			transactionData = transactionData.OrderBy(d => d.Date );
			var previousTransaction = transactionData.Take(1).First();
			IFundData previousFundData = null;
			foreach (var currentTransaction in transactionData.Skip(1))
			{
				var fundData = FundEntity.GetDataInRange(previousTransaction.Date, currentTransaction.Date);
				previousFundData = fundData.Take(1).FirstOrDefault();
				foreach (var fundDatum in fundData.Skip(1))
				{
					if (previousFundData.Date.Date <= previousTransaction.Date && previousTransaction.Date < fundDatum.Date.Date)
						Units += previousTransaction.Value / previousFundData.Value;

					AssestValues.Add(new FundData() { Value = Units * previousFundData.Value, Date = previousFundData.Date });
					previousFundData = fundDatum;
				}
				previousTransaction = currentTransaction;
			}
			//We must add on the last transactions units and any changes in value after
			var finalData = FundEntity.GetDataInRange(previousTransaction.Date, DateTime.Now);
			previousFundData = finalData.Take(1).FirstOrDefault();
			if (finalData.Count() == 1)
				Units += previousTransaction.Value / previousFundData.Value;

			foreach (var fundDatum in finalData.Skip(1))
			{
				if (previousFundData.Date.Date <= previousTransaction.Date && previousTransaction.Date < fundDatum.Date.Date)
					Units += previousTransaction.Value / previousFundData.Value;

				AssestValues.Add(new FundData() { Value = Units * previousFundData.Value, Date = previousFundData.Date });
				previousFundData = fundDatum;
			}
			//Finally the last change in value
			AssestValues.Add(new FundData() { Value = Units * previousFundData.Value, Date = previousFundData.Date });

		}



	}
}
