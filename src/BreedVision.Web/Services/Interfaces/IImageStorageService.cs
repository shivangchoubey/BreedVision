namespace BreedVision.Web.Services.Interfaces
{
    public interface IImageStorageService
    {
        Task<(bool IsValid, string? ErrorMessage)> ValidateImageAsync(IFormFile file);
        Task<string> SaveImageAsync(IFormFile file);
        bool DeleteImage(string relativePath);
    }
}
