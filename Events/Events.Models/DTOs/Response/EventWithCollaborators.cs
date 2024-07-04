using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Models.DTOs.Response
{
    public class EventWithCollaborators
    {
        public EventDTO eventDTO{ get; set; }

        public AccountDTO collaboratorDTO { get; set; }
    }
}
