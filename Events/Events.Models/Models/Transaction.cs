using Events.Utils;
using System;
using System.Collections.Generic;

namespace Events.Models.Models;

public partial class Transaction
{
    public string Id { get; set; } = null!;

    public string? RefId { get; set; } = null!;

    public DateTime? TransactionDate { get; set; }

    public double Amount { get; set; }

    public Enums.PaymentStatus PaymentStatus { get; set; }

    public string? PaymentMethod { get; set; } = null!;

    public string? VnPayTransactioId { get; set; } = null!;

    public string? ResponseCode { get; set; } = null!;

    public string? ResponseMessage { get; set; } = null!;

    public string? Description { get; set; }

    public string OrderId { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
