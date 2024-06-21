using Events.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        Task<bool> CreateTicket(Ticket ticket);
        Task<bool> CheckTicketExist(string email, string phoneNumber, int eventId);
    }
}
