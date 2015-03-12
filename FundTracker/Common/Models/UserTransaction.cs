using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
	[Table("UserTransaction")]
	public class UserTransaction : IFundData, IValidatableObject
	{
		[Key]
		[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
		public int UserTransactionId { get; set; }

		public int UserId { get; set; }

		[Display(Name = "Mutual Fund")]
		public String FundEntityId { get; set; }

		[ForeignKey("FundEntityId")]
		public virtual FundEntity FundEntity { get; set; }

		[ForeignKey("UserId")]
		public virtual UserProfile UserProfile { get; set; }

		public DateTime Date { get; set; }

		[Display(Name = "Cost of Shares Bought")]
		public float Value { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			using( var db = new DatabaseContext() )
			if (Date < db.Funds.Find(FundEntityId).FundHistory.First().Date)
				yield return new ValidationResult("We don't have any fund data that far back");
		}
	}
}
