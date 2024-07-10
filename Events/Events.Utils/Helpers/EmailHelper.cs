using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;

namespace Events.Utils.Helpers
{
    public class EmailHelper
    {
        public void SendEmailToNewAccount(string to, string username, string password)
        {
            //Create format
            string body = CreateEmailBody(username, password);

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("happycodingmice@gmail.com"));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = "Your account information";
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("vngo6790@gmail.com", "zsty sdbt azoc guoz");
            smtp.Send(email);
            smtp.Disconnect(true);
        }

		public void SendEmailToBuyTicketSuccess(string to, string filePath)
		{
			//Create format
			string body = CreateEmailBodyForTicket(filePath);

			var email = new MimeMessage();
			email.From.Add(MailboxAddress.Parse("vngo6790@gmail.com"));
			email.To.Add(MailboxAddress.Parse(to));
			email.Subject = "Your QRCode Infomation";
			var bodyBuilder = new BodyBuilder
			{
				HtmlBody = body
			};
			email.Body = bodyBuilder.ToMessageBody();

			using var smtp = new SmtpClient();
			smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
			smtp.Authenticate("vngo6790@gmail.com", "zsty sdbt azoc guoz");
			smtp.Send(email);
			smtp.Disconnect(true);
		}

		private string CreateEmailBodyForTicket(string filePath)
		{
			return $@"
            <html>
            <body>
                <p>Thank you for participating in our event</p>
                <p>Your Qrcode:</p>
                <img height=""224"" width=""225"" src=""{filePath}"" />

			</body>
            </html>";
		}

		private string CreateEmailBody(string username, string password)
        {
            return $@"
            <html>
            <body>
                <p>Here is your account:</p>
                <p>Username: {username}</p>
                <p>Password: {password}</p>
            </body>
            </html>";
        }
    }
}
