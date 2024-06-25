using Events.Models.DTOs.Request;
using Events.Models.Models;

namespace Events.Data.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        Task<string> CreateTicket(CreateTicketRequest request);
		Task UpdateOrderStatus(string orderId, string? responeCode);
        Task<IEnumerable<Ticket>> GetTicketFilter(bool isBought = false, string? orderId = null, string? includeProps = null);
	}
}
    