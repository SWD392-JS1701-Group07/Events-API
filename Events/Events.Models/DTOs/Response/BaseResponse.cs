using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Models.DTOs.Response
{
	public class BaseResponse
	{
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }
		public object? Data { get; set; } = null!;
		public IDictionary<string, string>? Errors { get; set; } = null!;
	}
}
