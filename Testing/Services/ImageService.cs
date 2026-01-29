using Pms.Service.Interface;
using Shared.Exceptions;

namespace Pms.Server.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _env;

        public ImageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string?> SaveImageAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                return null;

            if (file.Length > MaxFileSize)
                throw new InvalidOperationAppException("Image size cannot exceed 2MB.");

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!AllowedExtentions.Contains(extension))
                throw new InvalidOperationAppException(
                    "Only JPG, JPEG, PNG, and WEBP images are allowed."
                );

            var uploadsFolder = Path.Combine(
                _env.WebRootPath,
                "images",
                folderName
            );

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/images/{folderName}/{fileName}";
        }

        public void DeleteImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            var fullPath = Path.Combine(_env.WebRootPath, imagePath.TrimStart('/'));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        private static readonly string[] AllowedExtentions =
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        private const long MaxFileSize = 2 * 1024 * 1024; // 2MB
    }
}
