using System;
using System.Collections.Generic;

namespace Events.Models.Models;

public partial class Sponsorship
{
    public int Id { get; set; }

    public string? Description { get; set; }

    public string Type { get; set; } = null!;

    public string Title { get; set; } = null!;

    public double Sum { get; set; }

    public int SponsorId { get; set; }

    public int EventId { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual Sponsor Sponsor { get; set; } = null!;
}
