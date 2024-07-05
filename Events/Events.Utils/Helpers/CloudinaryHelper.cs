using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Utils.Helpers
{
    public class CloudinaryHelper
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryHelper(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream())
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return uploadResult.Url.ToString();
            }
            else
            {
                throw new Exception(uploadResult.Error.Message);
            }
        }

		public async Task<string> UploadImageForQrCodeAsync(Stream imageStream, string fileName)
		{
			var uploadParams = new ImageUploadParams()
			{
				File = new FileDescription(fileName, imageStream)
			};

			var uploadResult = await _cloudinary.UploadAsync(uploadParams);

			if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
			{
				return uploadResult.Url.ToString();
			}
			else
			{
				throw new Exception(uploadResult.Error.Message);
			}
		}
	}


    public class CloudinarySettings
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }
}
