using System;
using System.Collections.Generic;

namespace Events.Models.Models;

public partial class Order
{
    public string Id { get; set; } = null!;

    public DateOnly OrderDate { get; set; }

    public double TotalPrice { get; set; }

    public string? Notes { get; set; }

    public int OrderStatus { get; set; }

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public int? CustomerId { get; set; }

    public string? VnPayResponseCode { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
