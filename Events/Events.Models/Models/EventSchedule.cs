using System;
using System.Collections.Generic;

namespace Events.Models.Models;

public partial class EventSchedule
{
    public int Id { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string Place { get; set; } = null!;

    public int EventId { get; set; }

    public virtual Event Event { get; set; } = null!;
}
