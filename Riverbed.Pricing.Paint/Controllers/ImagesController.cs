using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;
using Microsoft.AspNetCore.Http;

namespace Riverbed.Pricing.Paint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public ImagesController(PricingDbContext context)
        {
            _context = context;
        }

        // DTO used for multipart/form-data upload to improve Swagger generation
        public class ImageUploadRequest
        {
            public IFormFile? File { get; set; }
            public string? ImageName { get; set; }
        }

        // GET: api/Images
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ImageAsset>>> GetAll()
        {
            return await _context.Images
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }

        // GET: api/Images/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ImageAsset>> GetById(Guid id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null) return NotFound();
            return image;
        }

        // GET: api/Images/{id}/content
        [HttpGet("{id:guid}/content")]
        public async Task<IActionResult> GetContent(Guid id)
        {
            var image = await _context.Images.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
            if (image == null || image.ImageData == null || image.ImageData.Length == 0)
                return NotFound();
            return File(image.ImageData, image.ContentType ?? "application/octet-stream", image.ImageName);
        }

        // POST: api/Images (multipart)
        [HttpPost]
        [RequestSizeLimit(20_000_000)]
        [Consumes("multipart/form-data")]
        [Produces("text/plain")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] ImageUploadRequest request)
        {
            if (request?.File == null)
            {
                return BadRequest("No file uploaded.");
            }

            await using var ms = new MemoryStream();
            await request.File.CopyToAsync(ms);
            var entity = new ImageAsset
            {
                Id = Guid.NewGuid(),
                ImageName = request.ImageName ?? request.File.FileName,
                ContentType = request.File.ContentType,
                ImageData = ms.ToArray(),
                CreatedAt = DateTime.UtcNow
            };
            _context.Images.Add(entity);
            await _context.SaveChangesAsync();

            var contentUrl = Url.Action(nameof(GetContent), values: new { id = entity.Id }) ?? $"https://bludog-software.com/rbp/api/Images/{entity.Id}/content";

            return Content(contentUrl, "text/plain");
        }

        // PUT: api/Images/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ImageAsset dto)
        {
            if (id != dto.Id) return BadRequest();
            var existing = await _context.Images.FindAsync(id);
            if (existing == null) return NotFound();

            existing.ImageName = dto.ImageName;
            existing.ContentType = dto.ContentType;
            if (dto.ImageData != null && dto.ImageData.Length > 0)
                existing.ImageData = dto.ImageData;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Images/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null) return NotFound();
            _context.Images.Remove(image);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
