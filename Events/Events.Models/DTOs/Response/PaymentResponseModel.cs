namespace Events.Models.DTOs.Response;

public class PaymentResponseModel
{
    public string OrderDescription { get; set; } = string.Empty;
    public string? RefId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public DateTime? PayDate { get; set; } 
    public string TransactionId { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;
	public string VnPayResponseCode { get; set; } = string.Empty;
}