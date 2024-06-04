using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Utils.Helper
{
	public static class ImageHelper
	{
		private const string ImageDirectory = "Images";
		public static async Task<string> SaveImageAsync(IFormFile image, IWebHostEnvironment environment)
		{
			var uploads = Path.Combine(environment.ContentRootPath, ImageDirectory);
			string fileName = new String(Path.GetFileNameWithoutExtension(image.FileName).Take(10).ToArray()).Replace(' ', '-');
			fileName = fileName + DateTime.Now.ToString("yyMMssfff") + Path.GetExtension(image.FileName);
			if (!Directory.Exists(uploads))
			{
				Directory.CreateDirectory(uploads);
			}

			var filePath = Path.Combine(uploads, fileName);
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await image.CopyToAsync(stream);
			}

			return $"/{ImageDirectory}/{fileName}";
		}

		public static void DeleteImage(string imageUrl, IWebHostEnvironment environment)
		{
			var filePath = Path.Combine(environment.ContentRootPath, imageUrl.TrimStart('/'));
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
		}
	}
}
