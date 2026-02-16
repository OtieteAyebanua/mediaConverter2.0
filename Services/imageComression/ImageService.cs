using FFMpegCore.Arguments;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats;

namespace mediaConverter.Services
{
    public class ImageService : IImageService
    {
        public async Task<FileContentResult> CompressJPGAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("No file provided");
            if (!image.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("This file format is not supported");
            await using var inputStream = new MemoryStream();
            await image.CopyToAsync(inputStream);
            inputStream.Position = 0;

            using var img = await Image.LoadAsync(inputStream);
            var encoder = new JpegEncoder { Quality = 75 };

            await using var outputStream = new MemoryStream();
            await img.SaveAsync(outputStream, encoder);
            outputStream.Position = 0;

            return new FileContentResult(outputStream.ToArray(), "image/jpeg")
            {
                FileDownloadName = Path.GetFileNameWithoutExtension(image.FileName + "_compressed.jpeg")
            };
        }

        public async Task<FileContentResult> CompressPNGAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("No file provided");

            if (!image.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Only PNG images are supported");

            await using var inputStream = new MemoryStream();
            await image.CopyToAsync(inputStream);
            inputStream.Position = 0;

            using var img = await Image.LoadAsync(inputStream);

            var encoder = new PngEncoder { CompressionLevel = PngCompressionLevel.BestCompression };

            await using var outputStream = new MemoryStream();
            await img.SaveAsync(outputStream, encoder);
            outputStream.Position = 0;

            return new FileContentResult(outputStream.ToArray(), "image/png")
            {
                FileDownloadName =
                    Path.GetFileNameWithoutExtension(image.FileName) + "_compressed.png",
            };
        }

        public async Task<FileContentResult> CompressGIFAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("No file provided");

            if (!image.FileName.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Only GIF images are supported");

            await using var inputStream = new MemoryStream();
            await image.CopyToAsync(inputStream);
            inputStream.Position = 0;

            using var img = await Image.LoadAsync(inputStream);
            var encoder = new GifEncoder
            {
                ColorTableMode = GifColorTableMode.Global,
                Quantizer = new SixLabors.ImageSharp.Processing.Processors.Quantization.WuQuantizer()
            };

            await using var outputStream = new MemoryStream();
            await img.SaveAsync(outputStream, encoder);
            outputStream.Position = 0;

            return new FileContentResult(outputStream.ToArray(), "image/gif")
            {
                FileDownloadName = Path.GetFileNameWithoutExtension(image.FileName) + "_compressed.gif"
            };
        }

        public async Task<FileContentResult> CompressWebPAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("No file provided");

            if (!image.FileName.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Only WebP images are supported");

            await using var inputStream = new MemoryStream();
            await image.CopyToAsync(inputStream);
            inputStream.Position = 0;

            using var img = await Image.LoadAsync(inputStream);
            var encoder = new WebpEncoder { Quality = 75 };

            await using var outputStream = new MemoryStream();
            await img.SaveAsync(outputStream, encoder);
            outputStream.Position = 0;

            return new FileContentResult(outputStream.ToArray(), "image/webp")
            {
                FileDownloadName = Path.GetFileNameWithoutExtension(image.FileName) + "_compressed.webp"
            };
        }

        public async Task<FileContentResult> CompressBMPAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("No file provided");

            if (!image.FileName.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Only BMP images are supported");

            await using var inputStream = new MemoryStream();
            await image.CopyToAsync(inputStream);
            inputStream.Position = 0;

            using var img = await Image.LoadAsync(inputStream);
            var encoder = new BmpEncoder();

            await using var outputStream = new MemoryStream();
            await img.SaveAsync(outputStream, encoder);
            outputStream.Position = 0;

            return new FileContentResult(outputStream.ToArray(), "image/bmp")
            {
                FileDownloadName = Path.GetFileNameWithoutExtension(image.FileName) + "_compressed.bmp"
            };
        }

        public async Task<FileContentResult> CompressImageAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("No file provided");

            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            await using var inputStream = new MemoryStream();
            await image.CopyToAsync(inputStream);
            inputStream.Position = 0;

            using var img = await Image.LoadAsync(inputStream);
            var quality = 75;

            IImageEncoder encoder = extension switch
            {
                ".jpg" or ".jpeg" => new JpegEncoder { Quality = quality },
                ".png" => new PngEncoder { CompressionLevel = PngCompressionLevel.BestCompression },
                ".gif" => new GifEncoder { ColorTableMode = GifColorTableMode.Global, Quantizer = new SixLabors.ImageSharp.Processing.Processors.Quantization.WuQuantizer() },
                ".webp" => new WebpEncoder { Quality = quality },
                ".bmp" => new BmpEncoder(),
                ".svg" => new JpegEncoder { Quality = quality },
                _ => throw new NotSupportedException($"Unsupported image format: {extension}")
            };

            var outputExtension = extension == ".svg" ? ".jpg" : extension;

            await using var outputStream = new MemoryStream();
            await img.SaveAsync(outputStream, encoder);
            outputStream.Position = 0;

            return new FileContentResult(outputStream.ToArray(), $"image/{outputExtension.TrimStart('.')}")
            {
                FileDownloadName = Path.GetFileNameWithoutExtension(image.FileName) + "_compressed" + outputExtension
            };
        }

        public async Task<FileContentResult> ConvertToFileResultAsync(IFormFile file, string targetExtension, int quality = 75)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file provided.", nameof(file));

            using var inputStream = file.OpenReadStream();
            using var image = await Image.LoadAsync(inputStream);

            IImageEncoder encoder = targetExtension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => new JpegEncoder { Quality = quality },
                ".png" => new PngEncoder(),
                ".webp" => new WebpEncoder { Quality = quality },
                ".gif" => new GifEncoder(),
                ".bmp" => new BmpEncoder(),
                ".tiff" => new TiffEncoder(),
                _ => throw new NotSupportedException($"Unsupported format: {targetExtension}")
            };

            await using var outputStream = new MemoryStream();
            await image.SaveAsync(outputStream, encoder);
            return new FileContentResult(outputStream.ToArray(), $"image/{targetExtension.TrimStart('.')}")
            {
                FileDownloadName = Path.GetFileNameWithoutExtension(file.FileName) + "_converted" + targetExtension
            };
        }

    }
}
