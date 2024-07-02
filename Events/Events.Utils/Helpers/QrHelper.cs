using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Utils.Helpers
{
	public class QrHelper
	{
		public static string GenerateQr(string data)
		{
			using (var qrGenerator = new QRCodeGenerator())
			{
				var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
				var qrCode = new QRCode(qrCodeData);
				using(Bitmap qrCodeImage = qrCode.GetGraphic(10))
				{
					using (Bitmap resizedQrCodeImage = new Bitmap(qrCodeImage, new Size(244, 245)))
					{
						using (MemoryStream ms = new MemoryStream())
						{
							resizedQrCodeImage.Save(ms, ImageFormat.Png);
							return Convert.ToBase64String(ms.ToArray());
						}
					}
				}
			}
		}
	}
}
