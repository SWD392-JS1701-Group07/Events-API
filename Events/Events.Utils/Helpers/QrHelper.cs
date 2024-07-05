using QRCoder;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Events.Utils.Helpers
{
	public class QrHelper
	{
		private readonly CloudinaryHelper _cloudinaryHelper;
		public QrHelper(CloudinaryHelper cloudinaryHelper)
		{
			_cloudinaryHelper = cloudinaryHelper;
		}
		public async Task<string> GenerateQr(string data, string fileName)
		{
			using (var qrGenerator = new QRCodeGenerator())
			{
				var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
				var qrCode = new BitmapByteQRCode(qrCodeData);
				byte[] qrCodeByte = qrCode.GetGraphic(20);
				// Convert byte[] to ImageSharp
				using (Image<Rgba32> qrCodeImage = Image.Load<Rgba32>(qrCodeByte))
				{
					qrCodeImage.Mutate(x => x.Resize(245, 245));
					// Save for format Png to Memory Stream
					using (MemoryStream ms = new MemoryStream())
					{
						qrCodeImage.Save(ms, new PngEncoder());
						ms.Position = 0;
						var qrUrl = await _cloudinaryHelper.UploadImageForQrCodeAsync(ms, $"{fileName}");
						return qrUrl;
					}
				}
			}
		}
	}
}


