﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Utils
{
	public class VnpaySettings
	{
        public string TmnCode { get; set; } = string.Empty;
		public string HashSecret { get; set; } = string.Empty;
		public string BaseUrl { get; set; } = string.Empty;
		public string Command { get; set; } = string.Empty;
		public string CurrCode { get; set; } = string.Empty;
		public string Version { get; set; } = string.Empty;
		public string Locale { get; set; } = string.Empty;
		public string TimeZoneId { get; set; } = string.Empty;
		public string ReturnUrl { get; set; } = string.Empty;
	}
}
