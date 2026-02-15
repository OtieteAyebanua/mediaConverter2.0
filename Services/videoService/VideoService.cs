using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FFMpegCore;
using FFMpegCore.Pipes;

namespace mediaConverter.Services
{

    public class VideoService : IVideoService
    {
        public async Task<FileContentResult> CompressMkvAsync(IFormFile videoFile, int crf = 28)
        {
            return await CompressVideoAsync(videoFile, ".mkv", crf);
        }

        public async Task<FileContentResult> CompressMovAsync(IFormFile videoFile, int crf = 28)
        {
            return await CompressVideoAsync(videoFile, ".mov", crf);
        }

        public async Task<FileContentResult> CompressMP4Async(IFormFile video, int crf = 28)
        {
            return await CompressVideoAsync(video, ".mp4", crf);
        }


        public async Task<FileContentResult> ConvertVideoAsync(IFormFile videoFile, string targetExtension, int crf = 28)
        {
            if (videoFile == null || videoFile.Length == 0)
                throw new ArgumentException("No video file provided", nameof(videoFile));
            targetExtension = targetExtension.StartsWith('.') ? targetExtension : "." + targetExtension;
            string[] allowedExtensions = { ".mp4", ".mkv", ".mov" };
            if (Array.IndexOf(allowedExtensions, targetExtension.ToLower()) < 0)
                throw new NotSupportedException($"Unsupported target format: {targetExtension}");
            var tempInput = Path.GetTempFileName() + Path.GetExtension(videoFile.FileName);
            var tempOutput = Path.GetTempFileName() + targetExtension;

            try
            {
                await using (var fs = new FileStream(tempInput, FileMode.Create))
                {
                    await videoFile.CopyToAsync(fs);
                }

                await FFMpegArguments
                    .FromFileInput(tempInput)
                    .OutputToFile(tempOutput, true, options => options
                        .WithVideoCodec("libx264")
                        .WithConstantRateFactor(crf)
                        .WithAudioCodec("aac")
                        .UsingMultithreading(true))
                    .ProcessAsynchronously();

                var outputBytes = await File.ReadAllBytesAsync(tempOutput);

                return new FileContentResult(outputBytes, $"video/{targetExtension.TrimStart('.')}")
                {
                    FileDownloadName = Path.GetFileNameWithoutExtension(videoFile.FileName) + "_converted" + targetExtension
                };
            }
            finally
            {
                if (File.Exists(tempInput)) File.Delete(tempInput);
                if (File.Exists(tempOutput)) File.Delete(tempOutput);
            }
        }


        private async Task<FileContentResult> CompressVideoAsync(IFormFile videoFile, string outputExtension, int crf)
        {
            if (videoFile == null || videoFile.Length == 0)
                throw new ArgumentException("No video file provided", nameof(videoFile));

            var tempInput = Path.GetTempFileName() + Path.GetExtension(videoFile.FileName);
            var tempOutput = Path.GetTempFileName() + outputExtension;

            try
            {
                await using (var fs = new FileStream(tempInput, FileMode.Create))
                {
                    await videoFile.CopyToAsync(fs);
                }

                await FFMpegArguments
                    .FromFileInput(tempInput)
                    .OutputToFile(tempOutput, true, options => options
                        .WithVideoCodec("libx264")
                        .WithConstantRateFactor(crf)
                        .WithAudioCodec("aac")
                        .UsingMultithreading(true))
                    .ProcessAsynchronously();

                var outputBytes = await File.ReadAllBytesAsync(tempOutput);

                return new FileContentResult(outputBytes, $"video/{outputExtension.TrimStart('.')}")
                {
                    FileDownloadName = Path.GetFileNameWithoutExtension(videoFile.FileName) + "_compressed" + outputExtension
                };
            }
            finally
            {
                if (File.Exists(tempInput)) File.Delete(tempInput);
                if (File.Exists(tempOutput)) File.Delete(tempOutput);
            }
        }
    }
}
