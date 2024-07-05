using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Models.DTOs
{
	public class SimpleTransactionDTO
	{
		public string Id { get; set; } = null!;

		public string? RefId { get; set; } = null!;

		public DateTime? TransactionDate { get; set; }

		public double Amount { get; set; }

		public string PaymentStatus { get; set; } = string.Empty;

		public string? PaymentMethod { get; set; } = null!;

		public string? VnPayTransactioId { get; set; } = null!;

		public string? ResponseCode { get; set; } = null!;

		public string? ResponseMessage { get; set; } = null!;

		public string? Description { get; set; }

		public string OrderId { get; set; } = null!;
	}
}
