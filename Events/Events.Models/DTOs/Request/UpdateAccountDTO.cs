using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Models.DTOs.Request
{
    public class UpdateAccountDTO
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? StudentId { get; set; }
        public DateTime Dob { get; set; }
        public string Gender { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public string AccountStatus { get; set; } = null!;
        public int? SubjectId { get; set; }
    }
}
