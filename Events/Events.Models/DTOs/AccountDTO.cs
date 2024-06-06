using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Models.DTOs
{
    public class AccountDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? StudentId { get; set; }

        public string? PhoneNumber { get; set; }

        public DateOnly? Dob { get; set; }

        public int Gender { get; set; }

        public string? AvatarUrl { get; set; }

        public int AccountStatus { get; set; }

        public int RoleId { get; set; }

        public int? SubjectId { get; set; }
    }
}
