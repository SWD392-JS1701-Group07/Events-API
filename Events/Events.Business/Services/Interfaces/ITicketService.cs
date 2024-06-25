using Events.Models.DTOs;
using Events.Models.DTOs.Request;
using Events.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Business.Services.Interfaces
{
    public interface ITicketService
    {
		Task<string> CreateTicket(CreateTicketRequest request);
		Task UpdateOrderStatus(string orderId, string? responeCode);
		Task<IEnumerable<TicketDTO>> GetTicketFilter(bool isBought = false, string? orderId = null, string? includeProps = null);
	}
}
