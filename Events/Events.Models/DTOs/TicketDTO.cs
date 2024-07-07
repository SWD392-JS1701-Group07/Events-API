
﻿using Events.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Events.Models.DTOs
{
	public class TicketDTO
	{
		public string Id { get; set; } = string.Empty;

		public string Name { get; set; } = null!;

		public string PhoneNumber { get; set; } = null!;

		public string? Qrcode { get; set; }

		public string Email { get; set; } = null!;

		public int EventId { get; set; }

		public double Price { get; set; }

		public string IsCheckIn { get; set; } = string.Empty;

		public string OrdersId { get; set; } = null!;
		public  EventDTO Event { get; set; } = null!;
	}


}
