using Events.Utils;
using System;
using System.Collections.Generic;

namespace Events.Models.Models;

public partial class Ticket
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Qrcode { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int EventId { get; set; }

    public double Price { get; set; }

    public Enums.IsCheckin IsCheckIn { get; set; }

    public int OrdersId { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual Order Orders { get; set; } = null!;
}
