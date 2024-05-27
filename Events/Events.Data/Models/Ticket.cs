using System;
using System.Collections.Generic;

namespace Events.Data.Models;

public partial class Ticket
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int EventId { get; set; }

    public double Price { get; set; }

    public int IsCheckIn { get; set; }

    public int OrderDetailsId { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual OrderDetail OrderDetails { get; set; } = null!;
}
