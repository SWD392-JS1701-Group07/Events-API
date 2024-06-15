using Events.Models.Models;
using Microsoft.AspNetCore.Http;
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
        public DateTime StartTimeOverall { get; set; }

        [Required]
        public DateTime EndTimeOverall { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        public string? AvatarUrl { get; set; }

        public string? Description { get; set; }


        [Required]
        public string EventStatus { get; set; }

        [Required]
        public int OwnerId { get; set; }

        public int? SubjectId { get; set; }

        public List<CreateEventScheduleDTO> ScheduleList { get; set; } = new List<CreateEventScheduleDTO>();

    }
}
