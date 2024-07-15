using Events.Business.Services.Interfaces;
using Events.Models.DTOs.Response;
using Events.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Events.Business.Services
{
    public class VNPayPaymentService : IVNPayPaymentService
    {
        private readonly VnpaySettings _vpnpaySettings;
        private readonly VnPayLibrary _vpnPayLibrary;

        public VNPayPaymentService(IOptions<VnpaySettings> config, VnPayLibrary vnPayLibrary)
        {
            _vpnpaySettings = config.Value;
            _vpnPayLibrary = vnPayLibrary;
        }

        public string CreatePayment(string amount, string orderInfo, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_vpnpaySettings.TimeZoneId);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            _vpnPayLibrary.AddRequestData("vnp_Version", _vpnpaySettings.Version);
			_vpnPayLibrary.AddRequestData("vnp_Command", _vpnpaySettings.Command);
			_vpnPayLibrary.AddRequestData("vnp_TmnCode", _vpnpaySettings.TmnCode);
			_vpnPayLibrary.AddRequestData("vnp_Amount", (int.Parse(amount) * 100).ToString());
            _vpnPayLibrary.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            _vpnPayLibrary.AddRequestData("vnp_CurrCode", _vpnpaySettings.CurrCode);
            _vpnPayLibrary.AddRequestData("vnp_IpAddr", _vpnPayLibrary.GetIpAddress(context));
			_vpnPayLibrary.AddRequestData("vnp_Locale", _vpnpaySettings.Locale);
            _vpnPayLibrary.AddRequestData("vnp_OrderInfo", orderInfo);
            _vpnPayLibrary.AddRequestData("vnp_OrderType", "electric");
			_vpnPayLibrary.AddRequestData("vnp_ReturnUrl", _vpnpaySettings.ReturnUrl);
            _vpnPayLibrary.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl =
				_vpnPayLibrary.CreateRequestUrl(_vpnpaySettings.BaseUrl, _vpnpaySettings.HashSecret);

            return paymentUrl;
        }
        
        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var response = _vpnPayLibrary.GetFullResponseData(collections, _vpnpaySettings.HashSecret);
            return response;
        }
    }
}
