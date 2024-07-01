using Events.Models;
using Events.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Models.DTOs
{
    public class EventDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartSellDate { get; set; }
        public DateTime EndSellDate { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int Remaining { get; set; }  
        public string? AvatarUrl { get; set; }
        public string? Description { get; set; }
        public string EventStatus { get; set; }
        public int OwnerId { get; set; }
        public int? SubjectId { get; set; }
        public List<EventScheduleDTO> ScheduleList { get; set; }
        public virtual ICollection<EventSchedule> Schedule { get; set; }
    }
}

