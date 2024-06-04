using Events.Models;
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
        public string Place { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Description { get; set; }
        public int EventStatus { get; set; }
        public int OwnerId { get; set; }
        public int? SubjectId { get; set; }
    }
}
