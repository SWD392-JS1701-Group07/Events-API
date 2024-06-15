using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Models.DTOs.Request
{
    public class CreateEventDTO
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Place { get; set; } = null!;
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public DateTime StartSellDate { get; set; }
        [Required]
        public DateTime EndSellDate { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public double Price { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        public string? Description { get; set; }
        [Required]
        public int EventStatus { get; set; }
        [Required]
        public int OwnerId { get; set; }
        public int? SubjectId { get; set; }
    }
}
