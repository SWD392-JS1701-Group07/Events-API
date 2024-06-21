using Events.Utils;
using System;
using System.Collections.Generic;

namespace Events.Models.Models;

public partial class Order
{
    public int Id { get; set; }

    public DateOnly OrderDate { get; set; }

    public float TotalPrice { get; set; }

    public string? Notes { get; set; }

    public Enums.PaymentMethod PaymentMethod { get; set; }

    public Enums.OrderStatus OrderStatus { get; set; }

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public int CustomerId { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
