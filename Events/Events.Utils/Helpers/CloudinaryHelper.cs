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

        public async Task<bool> ImageExistsAsync(string imageUrl)
        {
            try
            {
                var publicId = ExtractPublicIdFromUrl(imageUrl);
                if (string.IsNullOrEmpty(publicId))
                {
                    return false;
                }

                var getResult = await _cloudinary.GetResourceAsync(new GetResourceParams(publicId));
                return getResult.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            if (await ImageExistsAsync(publicId))
            {
                var deletionParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deletionParams);

                if (result.StatusCode == System.Net.HttpStatusCode.OK && result.Result == "ok")
                {
                    return true;
                }
                else
                {
                    throw new Exception(result.Error.Message);
                }
            }
            else
            {
                return false;
            }
        }

        private string ExtractPublicIdFromUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                var pathSegments = uri.AbsolutePath.Split('/');

                if (pathSegments.Length > 2 && pathSegments.Contains("upload"))
                {
                    var index = Array.IndexOf(pathSegments, "upload");
                    var publicIdWithFormat = pathSegments[index + 1]; 
                    var publicId = publicIdWithFormat.Substring(0, publicIdWithFormat.LastIndexOf('.'));
                    return publicId;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public class CloudinarySettings
        {
            public string CloudName { get; set; }
            public string ApiKey { get; set; }
            public string ApiSecret { get; set; }
        }
    }
}
