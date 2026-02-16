using Microsoft.AspNetCore.Mvc;

namespace mediaConverter.Services
{
    public interface IImageService
    {
        Task<FileContentResult> CompressPNGAsync(IFormFile image);
        Task<FileContentResult> CompressJPGAsync(IFormFile image);
        Task<FileContentResult> CompressGIFAsync(IFormFile image);
        Task<FileContentResult> CompressWebPAsync(IFormFile image);
        Task<FileContentResult> CompressBMPAsync(IFormFile image);
        Task<FileContentResult> CompressImageAsync(IFormFile image);
        Task<FileContentResult> ConvertToFileResultAsync(IFormFile file, string targetExtension, int quality = 75);
    }
}
