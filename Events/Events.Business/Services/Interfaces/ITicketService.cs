using Events.Models.DTOs.Request;
using Events.Models.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Business.Services.Interfaces
{
    public interface ITicketService
    {
        Task<BaseResponse> CreateFreeTicket(CreateTicketRequestDTO createTicketRequestDTO);
    }
}
