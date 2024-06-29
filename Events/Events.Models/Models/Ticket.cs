
using Events.Utils;
using System;
using System.Collections.Generic;

namespace Events.Models.Models;

public partial class Ticket
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? Qrcode { get; set; }

    public string Email { get; set; } = null!;

    public int EventId { get; set; }

    public double Price { get; set; }

    public Enums.IsCheckin IsCheckIn { get; set; }

    public string OrdersId { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;

    public virtual Order Orders { get; set; } = null!;
}