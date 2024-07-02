using Events.Models.DTOs.Response;
using Microsoft.AspNetCore.Http;

namespace Events.Business.Services.Interfaces;

public interface IVNPayPaymentService
{
    string CreatePayment(string amount, string orderInfo, HttpContext context);
    PaymentResponseModel PaymentExecute(IQueryCollection collections);
}