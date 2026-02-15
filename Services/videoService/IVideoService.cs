using Microsoft.AspNetCore.Mvc;

namespace mediaConverter.Services
{
    public interface IVideoService
    {
        Task<FileContentResult> CompressMP4Async(IFormFile video, int crf = 28);
        Task<FileContentResult> CompressMkvAsync(IFormFile videoFile, int crf = 28);
        Task<FileContentResult> CompressMovAsync(IFormFile videoFile, int crf = 28);
        Task<FileContentResult> ConvertVideoAsync(IFormFile videoFile, string targetExtension, int crf = 28);
    }
}