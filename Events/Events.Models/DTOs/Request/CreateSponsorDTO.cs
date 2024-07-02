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
		[Required]
		public string Name { get; set; }
		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public string Email { get; set; }
		[Required(ErrorMessage = "Phone Number is required")]
		[RegularExpression(@"^(0|\+84|84)(3|5|7|8|9)[0-9]{8}$", ErrorMessage = "Invalid phone number")]
		public string PhoneNumber { get; set; }
		public IFormFile? AvatarFile { get; set; } = null;
        public int? AccountId { get; set; }
	}
}
