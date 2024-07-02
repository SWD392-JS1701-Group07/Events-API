using System;
using System.Collections.Generic;

namespace Events.Models.Models;

public partial class Role
{
	public int Id { get; set; }

	public string Name { get; set; } = null!;

	public string? Description { get; set; }

	public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
