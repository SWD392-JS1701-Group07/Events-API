using Events.Utils;
using System;
using System.Collections.Generic;

namespace Events.Models.Models;

public partial class Event
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime StartSellDate { get; set; }

    public DateTime EndSellDate { get; set; }

    public double Price { get; set; }

    public int Quantity { get; set; }

    public int Remaining { get; set; }

    public string? AvatarUrl { get; set; }

    public string? Description { get; set; }

    public Enums.EventStatus EventStatus { get; set; }

    public int OwnerId { get; set; }

    public int? SubjectId { get; set; }

    public virtual ICollection<Collaborator> Collaborators { get; set; } = new List<Collaborator>();

    public virtual ICollection<EventSchedule> EventSchedules { get; set; } = new List<EventSchedule>();

    public virtual Account Owner { get; set; } = null!;

    public virtual ICollection<Sponsorship> Sponsorships { get; set; } = new List<Sponsorship>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
