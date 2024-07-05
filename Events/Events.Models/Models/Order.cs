using Events.Utils;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Events.Models.Models;

public partial class Order
{
    public string Id { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public double TotalPrice { get; set; }

    public string? Notes { get; set; }

    public Enums.OrderStatus OrderStatus { get; set; }

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public int? CustomerId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
