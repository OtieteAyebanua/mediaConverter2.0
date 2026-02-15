using Microsoft.AspNetCore.Mvc;

namespace mediaConverter.Services
{
    public interface ICoordinator
    {
        Task<FileContentResult> CoordinateImageCompression(IFormFile image, string extension);
        Task<FileContentResult> CoordinateImageConversion(IFormFile image, string targetExtension);
        Task<FileContentResult> CoordinateVideoCompression(IFormFile video, string extension, int crf = 28);
        Task<FileContentResult> CoordinateVideoConversion(IFormFile video, string extension, int crf = 28);
        Task<FileContentResult> CoordinateDocumentCompression(IFormFile docs, string extension);
        Task<FileContentResult> CoordinateDocumentConversion(IFormFile docs, string extension);
    }
}