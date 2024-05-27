using Events.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.Interfaces
{
    public interface IEventRepository
    {
        Task<List<Event>> GetAllEvents();
    }
}
