using Events.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Models.DTOs.Request
{
    public class CreateAccountDTO
    {
        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string? Password { get; set; }
        public string? StudentId { get; set; }

        public string? PhoneNumber { get; set; }

        public DateTime Dob { get; set; }

        public string Gender { get; set; }

        public string? AvatarUrl { get; set; }

        public int RoleId { get; set; }

        public int? SubjectId { get; set; }
    }
}
