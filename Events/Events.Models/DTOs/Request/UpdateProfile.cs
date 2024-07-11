using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Models.DTOs.Request
{
    public class UpdateProfile
    {
        public string Name { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? StudentId { get; set; }
        public DateTime Dob { get; set; }
        public string Gender { get; set; } = null!;
        public int? SubjectId { get; set; }
    }
}
