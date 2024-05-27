using System;
using System.Collections.Generic;

namespace Events.Data.Models;

public partial class Transaction
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int Method { get; set; }

    public DateTime Date { get; set; }

    public virtual Order Order { get; set; } = null!;
}
