using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Models.DTOs.Request
{
	public class CreateTicketRequest
	{
		public string PaymentType { get; set; }
		public string OrderNotes { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public double TotalAmount { get; set; }
		public int CustomerId { get; set; }
		public List<TicketDetail> Tickets { get; set; }
	}

	public class TicketDetail
	{
		public string Name { get; set; }
		public string PhoneNumber { get; set; }
		public string Email { get; set; }
		public int EventId { get; set; }
	}
}
