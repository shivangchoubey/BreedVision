using BreedVision.Web.Services.Interfaces;

namespace BreedVision.Web.Services.Implementations
{
    public class LocalFileStorageService : IImageStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<LocalFileStorageService> _logger;
        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB

        private static readonly Dictionary<string, List<byte[]>> FileSignatures = new()
        {
            { ".jpg", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF } } },
            { ".jpeg", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF } } },
            { ".png", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47 } } },
            { ".webp", new List<byte[]> { new byte[] { 0x52, 0x49, 0x46, 0x46 } } },
            { ".bmp", new List<byte[]> { new byte[] { 0x42, 0x4D } } }
        };

        public LocalFileStorageService(IWebHostEnvironment env, ILogger<LocalFileStorageService> logger)
        {
            _env = env;
            _logger = logger;
        }

        public async Task<(bool IsValid, string? ErrorMessage)> ValidateImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return (false, "Please select an image file to upload.");
            }

            if (file.Length > MaxFileSizeBytes)
            {
                return (false, $"File size exceeds the 10 MB maximum limit (File size: {file.Length / (1024 * 1024.0):F2} MB).");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !FileSignatures.ContainsKey(extension))
            {
                return (false, "Unsupported file format. Please upload a valid JPG, PNG, WEBP, or BMP image.");
            }

            // Verify Magic Bytes
            try
            {
                using var stream = file.OpenReadStream();
                using var reader = new BinaryReader(stream);
                var headerBytes = reader.ReadBytes(8);

                bool validMagicBytes = false;
                var signatures = FileSignatures[extension];
                foreach (var signature in signatures)
                {
                    if (headerBytes.Take(signature.Length).SequenceEqual(signature))
                    {
                        validMagicBytes = true;
                        break;
                    }
                }

                if (!validMagicBytes)
                {
                    return (false, "The file header does not match a valid image format. The file may be corrupted.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read header bytes from {FileName}", file.FileName);
                return (false, "Could not verify the image content integrity.");
            }

            return await Task.FromResult((true, (string?)null));
        }

        public async Task<string> SaveImageAsync(IFormFile file)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var uniqueFileName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(uploadsDir, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation("Saved uploaded image to {Path}", fullPath);
            return $"/uploads/{uniqueFileName}";
        }

        public bool DeleteImage(string relativePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(relativePath)) return false;

                var cleanPath = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var fullPath = Path.Combine(_env.WebRootPath, cleanPath);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation("Deleted image at {Path}", fullPath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete image at {Path}", relativePath);
            }

            return false;
        }
    }
}
