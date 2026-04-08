using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Riverbed.Pricing.Paint.Controllers.Utils
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomImageController : ControllerBase
    {
        private readonly PricingDbContext _context;
        private readonly ILogger<RoomImageController> _logger;
        private const int MaxImageWidth = 1920;
        private const int MaxImageHeight = 1080;
        private const int ThumbnailSize = 200;
        private const int JpegQuality = 85;

        public RoomImageController(PricingDbContext context, ILogger<RoomImageController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Upload and compress an image for a room
        /// </summary>
        [HttpPost("Upload/{roomId:int}")]
        public async Task<IActionResult> UploadRoomImage(int roomId, IFormFile file)
        {
            ArgumentNullException.ThrowIfNull(file);

            if (file.Length == 0)
                return BadRequest("No file uploaded.");

            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
            if (room == null)
                return NotFound($"Room with ID {roomId} not found.");

            try
            {
                // Load and compress the image
                using var imageStream = file.OpenReadStream();
                using var image = await Image.LoadAsync(imageStream);

                // Resize if needed
                if (image.Width > MaxImageWidth || image.Height > MaxImageHeight)
                {
                    var ratioX = (double)MaxImageWidth / image.Width;
                    var ratioY = (double)MaxImageHeight / image.Height;
                    var ratio = Math.Min(ratioX, ratioY);

                    var newWidth = (int)(image.Width * ratio);
                    var newHeight = (int)(image.Height * ratio);

                    image.Mutate(x => x.Resize(newWidth, newHeight));
                }

                // Save compressed image to memory
                using var compressedStream = new MemoryStream();
                await image.SaveAsync(compressedStream, new JpegEncoder { Quality = JpegQuality });
                var compressedBytes = compressedStream.ToArray();

                // Delete old image if exists
                if (room.RoomImageId.HasValue)
                {
                    var oldImage = await _context.DataFiles.FirstOrDefaultAsync(f => f.Id == room.RoomImageId.Value);
                    if (oldImage != null)
                    {
                        _context.DataFiles.Remove(oldImage);
                    }
                }

                // Create new DataFile entry
                var dataFile = new DataFile
                {
                    FileName = file.FileName,
                    FileType = "image/jpeg",
                    FileData = compressedBytes,
                    FileSize = compressedBytes.Length,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true,
                    Description = $"Room image for {room.Name}"
                };

                _context.DataFiles.Add(dataFile);
                await _context.SaveChangesAsync();

                // Update room with new image ID
                room.RoomImageId = dataFile.Id;
                await _context.SaveChangesAsync();

                var baseUri = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
                return Ok(new
                {
                    id = dataFile.Id,
                    roomId = room.Id,
                    url = $"{baseUri}/api/RoomImage/Download/{dataFile.Id}",
                    thumbnailUrl = $"{baseUri}/api/RoomImage/Thumbnail/{dataFile.Id}",
                    originalSize = file.Length,
                    compressedSize = compressedBytes.Length
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading room image for room {RoomId}", roomId);
                return StatusCode(500, $"Error processing image: {ex.Message}");
            }
        }

        /// <summary>
        /// Download full-size room image
        /// </summary>
        [HttpGet("Download/{imageId:int}")]
        public async Task<IActionResult> DownloadRoomImage(int imageId)
        {
            var dataFile = await _context.DataFiles.FirstOrDefaultAsync(f => f.Id == imageId);
            if (dataFile == null)
                return NotFound($"Image with ID {imageId} not found.");

            return File(dataFile.FileData, dataFile.FileType ?? "image/jpeg", dataFile.FileName);
        }

        /// <summary>
        /// Get thumbnail version of room image
        /// </summary>
        [HttpGet("Thumbnail/{imageId:int}")]
        public async Task<IActionResult> GetThumbnail(int imageId)
        {
            var dataFile = await _context.DataFiles.FirstOrDefaultAsync(f => f.Id == imageId);
            if (dataFile == null)
                return NotFound($"Image with ID {imageId} not found.");

            try
            {
                using var imageStream = new MemoryStream(dataFile.FileData);
                using var image = await Image.LoadAsync(imageStream);

                // Create square thumbnail
                var size = Math.Min(image.Width, image.Height);
                image.Mutate(x => x
                    .Resize(new ResizeOptions
                    {
                        Size = new Size(ThumbnailSize, ThumbnailSize),
                        Mode = ResizeMode.Crop
                    }));

                using var thumbnailStream = new MemoryStream();
                await image.SaveAsync(thumbnailStream, new JpegEncoder { Quality = 80 });
                var thumbnailBytes = thumbnailStream.ToArray();

                return File(thumbnailBytes, "image/jpeg");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating thumbnail for image {ImageId}", imageId);
                return StatusCode(500, $"Error generating thumbnail: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete room image
        /// </summary>
        [HttpDelete("Delete/{roomId:int}")]
        public async Task<IActionResult> DeleteRoomImage(int roomId)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
            if (room == null)
                return NotFound($"Room with ID {roomId} not found.");

            if (!room.RoomImageId.HasValue)
                return BadRequest("Room has no image to delete.");

            var dataFile = await _context.DataFiles.FirstOrDefaultAsync(f => f.Id == room.RoomImageId.Value);
            if (dataFile != null)
            {
                _context.DataFiles.Remove(dataFile);
            }

            room.RoomImageId = null;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Room image deleted successfully" });
        }

        /// <summary>
        /// Get room image info
        /// </summary>
        [HttpGet("Info/{roomId:int}")]
        public async Task<IActionResult> GetRoomImageInfo(int roomId)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
            if (room == null)
                return NotFound($"Room with ID {roomId} not found.");

            if (!room.RoomImageId.HasValue)
                return Ok(new { hasImage = false });

            var dataFile = await _context.DataFiles.FirstOrDefaultAsync(f => f.Id == room.RoomImageId.Value);
            if (dataFile == null)
                return Ok(new { hasImage = false });

            var baseUri = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            return Ok(new
            {
                hasImage = true,
                id = dataFile.Id,
                fileName = dataFile.FileName,
                fileSize = dataFile.FileSize,
                createdDate = dataFile.CreatedDate,
                url = $"{baseUri}/api/RoomImage/Download/{dataFile.Id}",
                thumbnailUrl = $"{baseUri}/api/RoomImage/Thumbnail/{dataFile.Id}"
            });
        }
    }
}
