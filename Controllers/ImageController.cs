using Microsoft.AspNetCore.Mvc;
using mediaConverter.Services;

namespace mediaConverter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly ICoordinator _coordinator;
        private readonly ILogger<ImageController> _logger;

        public ImageController(ICoordinator coordinator, ILogger<ImageController> logger)
        {
            _coordinator = coordinator;
            _logger = logger;
        }

        /// <summary>
        /// Compress an image file
        /// </summary>
        /// <param name="file">Image file to compress</param>
        /// <returns>Compressed image file</returns>
        [IgnoreAntiforgeryToken]
        [HttpPost("compress")]
        public async Task<IActionResult> CompressImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file provided");
                }

                var extension = Path.GetExtension(file.FileName).TrimStart('.').ToLower();
                
                if (string.IsNullOrEmpty(extension))
                {
                    return BadRequest("Unable to determine file extension");
                }

                var compressedFile = await _coordinator.CoordinateImageCompression(file, extension);

                if (compressedFile == null)
                {
                    return BadRequest("Failed to compress image");
                }

                return compressedFile;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"Image compression error: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error during image compression: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        /// <summary>
        /// Convert image to a different format
        /// </summary>
        /// <param name="file">Image file to convert</param>
        /// <param name="targetFormat">Target image format (jpg, png, webp, gif, bmp, tiff)</param>
        /// <returns>Converted image file</returns>
        [IgnoreAntiforgeryToken]
        [HttpPost("convert")]
        public async Task<IActionResult> ConvertImage([FromForm(Name = "ImageFile")] IFormFile file, [FromForm(Name = "TargetFormat")] string targetFormat)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file provided");
                }

                if (string.IsNullOrEmpty(targetFormat))
                {
                    return BadRequest("Target format is required");
                }

                var targetExtension = targetFormat.StartsWith('.') ? targetFormat : $".{targetFormat}";
                var convertedFile = await _coordinator.CoordinateImageConversion(file, targetExtension);

                if (convertedFile == null)
                {
                    return BadRequest("Failed to convert image");
                }

                return convertedFile;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"Image conversion error: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (NotSupportedException ex)
            {
                _logger.LogError($"Unsupported format: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error during image conversion: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred");
            }
        }
    }
}
