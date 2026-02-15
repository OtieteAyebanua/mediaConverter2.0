using Microsoft.AspNetCore.Mvc;
using mediaConverter.Services;

namespace mediaConverter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoController : ControllerBase
    {
        private readonly ICoordinator _coordinator;
        private readonly ILogger<VideoController> _logger;

        public VideoController(ICoordinator coordinator, ILogger<VideoController> logger)
        {
            _coordinator = coordinator;
            _logger = logger;
        }

        /// <summary>
        /// Compress a video file
        /// </summary>
        /// <param name="file">Video file to compress</param>
        /// <param name="crf">Quality factor (0-51, lower = better quality, default 28)</param>
        /// <returns>Compressed video file</returns>
        [IgnoreAntiforgeryToken]
        [HttpPost("compress")]
        public async Task<IActionResult> CompressVideo([FromForm(Name = "VideoFile")] IFormFile file, [FromForm(Name = "CrfValue")] int crf = 28)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file provided");
                }

                if (crf < 0 || crf > 51)
                {
                    return BadRequest("CRF value must be between 0 and 51");
                }

                var extension = Path.GetExtension(file.FileName).TrimStart('.').ToLower();
                
                if (string.IsNullOrEmpty(extension))
                {
                    return BadRequest("Unable to determine file extension");
                }

                var compressedFile = await _coordinator.CoordinateVideoCompression(file, extension, crf);

                if (compressedFile == null)
                {
                    return BadRequest("Failed to compress video");
                }

                return compressedFile;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"Video compression error: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (NotSupportedException ex)
            {
                _logger.LogError($"Unsupported format: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error during video compression: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        /// <summary>
        /// Convert video to a different format
        /// </summary>
        /// <param name="file">Video file to convert</param>
        /// <param name="targetFormat">Target video format (mp4, mkv, mov)</param>
        /// <param name="crf">Quality factor (0-51, lower = better quality, default 28)</param>
        /// <returns>Converted video file</returns>
        [IgnoreAntiforgeryToken]
        [HttpPost("convert")]
        public async Task<IActionResult> ConvertVideo(IFormFile file, [FromQuery] string targetFormat, [FromQuery] int crf = 28)
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

                if (crf < 0 || crf > 51)
                {
                    return BadRequest("CRF value must be between 0 and 51");
                }

                var targetExtension = targetFormat.StartsWith('.') ? targetFormat : $".{targetFormat}";
                var convertedFile = await _coordinator.CoordinateVideoConversion(file, targetExtension, crf);

                if (convertedFile == null)
                {
                    return BadRequest("Failed to convert video");
                }

                return convertedFile;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"Video conversion error: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (NotSupportedException ex)
            {
                _logger.LogError($"Unsupported format: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error during video conversion: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred");
            }
        }
    }
}
