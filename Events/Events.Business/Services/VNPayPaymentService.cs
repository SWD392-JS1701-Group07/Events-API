using Events.Business.Services.Interfaces;
using Events.Models.DTOs.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Events.Business.Services
{
    public class VNPayPaymentService : IVNPayPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly VnPayLibrary _vpnPayLibrary;

        public VNPayPaymentService(IConfiguration configuration, VnPayLibrary vnPayLibrary)
        {
            _configuration = configuration;
            _vpnPayLibrary = vnPayLibrary;
        }

        public string CreatePayment(string amount, string orderInfo, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];
            _vpnPayLibrary.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
			_vpnPayLibrary.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
			_vpnPayLibrary.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
			_vpnPayLibrary.AddRequestData("vnp_Amount", (int.Parse(amount) * 100).ToString());
            _vpnPayLibrary.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            _vpnPayLibrary.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            _vpnPayLibrary.AddRequestData("vnp_IpAddr", _vpnPayLibrary.GetIpAddress(context));
			_vpnPayLibrary.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            _vpnPayLibrary.AddRequestData("vnp_OrderInfo", orderInfo);
            _vpnPayLibrary.AddRequestData("vnp_OrderType", "electric");
			_vpnPayLibrary.AddRequestData("vnp_ReturnUrl", urlCallBack);
            _vpnPayLibrary.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl =
				_vpnPayLibrary.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            return paymentUrl;
        }
        
        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var response = _vpnPayLibrary.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

            return response;
        }
    }
}
