using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Models.DTOs
{
    public class SubjectDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}
