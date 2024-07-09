using Events.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Models.DTOs
{
    public class CollaboratorDTO
    {
        public int Id { get; set; }
        public int IsCheckIn { get; set; }
        public int AccountId { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string CollabStatus { get; set; }
        public string? Task { get; set; }
        public string? Description { get; set; }
    }
}
