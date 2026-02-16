using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;

namespace mediaConverter.Services
{
    public class Coordinator : ICoordinator
    {
        private IImageService _imageService;
        private IVideoService _videoService;

        public Coordinator(IImageService imageService, IVideoService videoService)
        {
            this._imageService = imageService;
            this._videoService = videoService;
        }

        public Task<FileContentResult> CoordinateDocumentCompression(IFormFile docs, string extension)
        {
            throw new NotImplementedException();
        }

        public Task<FileContentResult> CoordinateDocumentConversion(IFormFile docs, string extension)
        {
            throw new NotImplementedException();
        }

        public async Task<FileContentResult> CoordinateImageCompression(IFormFile image, string extension)
        {
            if (extension == null || image == null || image.Length == 0)
                return null;

            return await _imageService.CompressImageAsync(image);
        }

        public async Task<FileContentResult> CoordinateImageConversion(IFormFile image, string targetExtension)
        {
            if (targetExtension == null || image == null || image.Length == 0)
                return null;

            return await _imageService.ConvertToFileResultAsync(image, targetExtension);
        }

        public async Task<FileContentResult> CoordinateVideoCompression(IFormFile video, string extension, int crf = 28)
        {
            if (extension == null || video == null || video.Length == 0)
                return null;

            string type = extension.ToLower();
            FileContentResult compressedVideo = null;

            switch (type)
            {
                case "mp4":
                    compressedVideo = await _videoService.CompressMP4Async(video, crf);
                    break;

                case "mkv":
                    compressedVideo = await _videoService.CompressMkvAsync(video, crf);
                    break;

                case "mov":
                    compressedVideo = await _videoService.CompressMovAsync(video, crf);
                    break;

                default:
                    throw new ArgumentException("Unsupported video format");
            }

            return compressedVideo;
        }

        public async Task<FileContentResult> CoordinateVideoConversion(IFormFile video, string extension, int crf = 28)
        {
            if (extension == null || video == null || video.Length == 0)
                return null;

            return await _videoService.ConvertVideoAsync(video, extension, crf);
        }
    }
}
