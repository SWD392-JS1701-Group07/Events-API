using Events.Utils;
using System;
using System.Collections.Generic;

namespace Events.Models.Models;

public partial class Account
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? StudentId { get; set; }

    public string? PhoneNumber { get; set; }

    public DateOnly Dob { get; set; }

    public Enums.Gender Gender { get; set; }

    public string? AvatarUrl { get; set; }

    public Enums.AccountStatus AccountStatus { get; set; }

    public int RoleId { get; set; }

    public int? SubjectId { get; set; }

    public virtual ICollection<Collaborator> Collaborators { get; set; } = new List<Collaborator>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual Role Role { get; set; } = null!;

    public virtual Subject? Subject { get; set; }
}
