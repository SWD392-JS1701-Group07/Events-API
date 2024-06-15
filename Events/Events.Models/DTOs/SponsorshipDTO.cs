﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Models.DTOs
{
    public class SponsorshipDTO
    {
        public int Id { get; set; }

        public string? Description { get; set; }

        public string Type { get; set; } = null!;

        public string Title { get; set; } = null!;

        public double Sum { get; set; }

        public int SponsorId { get; set; }

        public int EventId { get; set; }
    }
}
