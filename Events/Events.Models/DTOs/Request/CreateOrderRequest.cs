using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Models.DTOs.Request
{
	public class CreateOrderRequest
	{
		public string OrderNotes { get; set; } = string.Empty;
		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public string Email { get; set; } = string.Empty;
		[Required(ErrorMessage = "Phone Number is required")]
		[RegularExpression(@"^(0|\+84|84)(3|5|7|8|9)[0-9]{8}$", ErrorMessage = "Invalid phone number")]
		public string PhoneNumber { get; set; } = string.Empty;
		[Required(ErrorMessage = "TotalAmount is required")]
		[RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = "TotalAmount must be a number")]
		public double TotalAmount { get; set; }
		public int? CustomerId { get; set; }
		public List<TicketDetail> Tickets { get; set; } = new List<TicketDetail>();
	}
	public class TicketDetail
	{
		[Required]
		public string Name { get; set; } = string.Empty;
		[Required(ErrorMessage = "Phone Number is required")]
		[RegularExpression(@"^(0|\+84|84)(3|5|7|8|9)[0-9]{8}$", ErrorMessage = "Invalid phone number")]
		public string PhoneNumber { get; set; } = string.Empty;
		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public string Email { get; set; } = string.Empty;
		[Required]
		public int EventId { get; set; }
	}
}
