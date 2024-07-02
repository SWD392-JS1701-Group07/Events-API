namespace Events.Models.DTOs.Response;

public class PaymentResponseModel
{
    public string OrderDescription { get; set; }
    public string? RefId { get; set; }
    public DateTime? PayDate { get; set; }
    public string TransactionId { get; set; }
    public string PaymentMethod { get; set; }
    public bool Success { get; set; }
    public string Token { get; set; }
    public string VnPayResponseCode { get; set; }
}