using Events.Utils;
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
		[RegularExpression(RegexBase.PhoneNumberRegex, ErrorMessage = "Invalid phone number")]
		public string PhoneNumber { get; set; } = string.Empty;
		[Required(ErrorMessage = "TotalAmount is required")]
		[RegularExpression(RegexBase.MustNumberRegex, ErrorMessage = "TotalAmount must be a number")]
		public double TotalAmount { get; set; }
		public int? CustomerId { get; set; }
		public List<TicketDetail> Tickets { get; set; } = new List<TicketDetail>();
	}
	public class TicketDetail
	{
		[Required(ErrorMessage = "Name is required")]
		[RegularExpression(RegexBase.SingleSpaceCharacterRegex, ErrorMessage = $"Name {RegexBase.ErrorMessageSingleSpaceCharacterRegex}")]
		public string Name { get; set; } = string.Empty;
		[Required(ErrorMessage = "Phone Number is required")]
		[RegularExpression(RegexBase.PhoneNumberRegex, ErrorMessage = "Invalid phone number")]
		public string PhoneNumber { get; set; } = string.Empty;
		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public string Email { get; set; } = string.Empty;
		[Required(ErrorMessage = "Price is required")]
		[RegularExpression(RegexBase.MustNumberRegex, ErrorMessage = "Price must be a number")]
		public double Price { get; set; }
		[Required(ErrorMessage = "EventId is required")]
		public int EventId { get; set; }
	}
}
