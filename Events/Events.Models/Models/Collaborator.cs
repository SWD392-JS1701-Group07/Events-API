using Events.Utils;
using System;
using System.Collections.Generic;

namespace Events.Models.Models;

public partial class Collaborator
{
	public int Id { get; set; }

	public int IsCheckIn { get; set; }

	public int AccountId { get; set; }

	public int EventId { get; set; }

	public Enums.CollaboratorStatus CollabStatus { get; set; }

	public string? Task { get; set; }

	public string? Description { get; set; }

	public virtual Account Account { get; set; } = null!;

	public virtual Event Event { get; set; } = null!;
}
