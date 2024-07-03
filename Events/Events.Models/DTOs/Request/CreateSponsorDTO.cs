using Events.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Models.DTOs.Request
{
	public class CreateSponsorDTO
	{
		[Required(ErrorMessage = "Name is required")]
		[RegularExpression(RegexBase.SingleSpaceCharacterRegex, ErrorMessage = $"Name {RegexBase.ErrorMessageSingleSpaceCharacterRegex}")]
		public string Name { get; set; } = string.Empty;
		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public string Email { get; set; } = string.Empty;
		[Required(ErrorMessage = "Phone Number is required")]
		[RegularExpression(RegexBase.PhoneNumberRegex, ErrorMessage = "Invalid phone number")]
		public string PhoneNumber { get; set; } = string.Empty;
		public IFormFile? AvatarFile { get; set; } = null;
        public int? AccountId { get; set; }
	}
}
