using Events.Models.Models;
using Events.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Events.Models.DTOs
{
    public class OrderDTO
    {
        public string Id { get; set; }

        public DateTime OrderDate { get; set; }

        public float TotalPrice { get; set; }

        public string? Notes { get; set; }

        public string PaymentMethod { get; set; }

        public string OrderStatus { get; set; }

        public string Email { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public int CustomerId { get; set; }
		public virtual CustomerDTO? Customer { get; set; }
		public virtual List<SimpleTicketDTO> Tickets { get; set; } = new List<SimpleTicketDTO>();
		public virtual List<SimpleTransactionDTO> Transactions { get; set; } = new List<SimpleTransactionDTO>();

	}
}
