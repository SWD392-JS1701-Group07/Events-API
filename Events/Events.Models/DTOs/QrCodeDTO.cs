using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Models.DTOs
{
	public class QrCodeDTO
	{
		public string TicketId { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public double Price {  get; set; }
		public string OrderId { get; set; } = string.Empty;
		public string EventName {  get; set; } = string.Empty;


	}
}
