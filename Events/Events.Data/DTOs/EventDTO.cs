using Events.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Data.DTOs
{
    public class EventDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Place { get; set; } = null!;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? Description { get; set; }

        public string EventStatus { get; set; }

        public int OwnerId { get; set; }

        public int? SubjectId { get; set; }
    }
}
