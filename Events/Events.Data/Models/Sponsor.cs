using System;
using System.Collections.Generic;

namespace Events.Data.Models;

public partial class Sponsor
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public int? AccountId { get; set; }

    public virtual ICollection<Sponsorship> Sponsorships { get; set; } = new List<Sponsorship>();
}
