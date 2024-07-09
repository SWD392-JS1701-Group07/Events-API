using Events.Models.Models;
using Events.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Models.DTOs
{
	public class SimpleOrderDTO
	{
		public string Id { get; set; } = null!;

		public DateTime OrderDate { get; set; }

		public double TotalPrice { get; set; }

		public string? Notes { get; set; }

		public string OrderStatus { get; set; } = string.Empty;

		public string Email { get; set; } = null!;

		public string PhoneNumber { get; set; } = null!;

		public int? CustomerId { get; set; }
	}
}
