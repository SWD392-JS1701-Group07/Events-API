using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Models.DTOs
{
    public class TicketDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string Qrcode { get; set; } = null!;

        public string Email { get; set; } = null!;

        public int EventId { get; set; }

        public double Price { get; set; }

        public string IsCheckIn { get; set; }

        public int OrdersId { get; set; }
    }
}
