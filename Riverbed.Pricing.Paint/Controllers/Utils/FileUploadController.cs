using Microsoft.AspNetCore.Mvc;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace Riverbed.Pricing.Paint.Controllers.Utils
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileManagerController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public FileManagerController(PricingDbContext context)
        {
            _context = context;
        }

        [HttpPost("Upload/{projectGuid}")]
        public async Task<IActionResult> Upload(IFormFile file, string projectGuid)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Read file into byte array
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            // Create a database record
            var fileEntity = new DataFile
            {
                FileName = file.FileName,
                FileType = file.ContentType,
                FileData = fileBytes,
                FileSize = file.Length,
                CreatedDate = DateTime.UtcNow
            };

            _context.DataFiles.Add(fileEntity);
            await _context.SaveChangesAsync();

            // Create a project association with the uploaded file
            var companyFile = new CompanyFile
            {
                DataFileId = fileEntity.Id,
                CompanyGuid = Guid.Parse(projectGuid),
            };

            _context.CompanyFiles.Add(companyFile);
            await _context.SaveChangesAsync();

            // Build absolute download URL for the caller
            var baseUri = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var downloadUrl = $"{baseUri}/api/FileManager/Download/{fileEntity.Id}";

            return Ok(new { id = fileEntity.Id, url = downloadUrl });
        }

        // GET: api/FileManager/Download/{id}
        [HttpGet("Download/{id:int}")]
        public async Task<IActionResult> Download(int id)
        {
            var file = await _context.DataFiles.FirstOrDefaultAsync(f => f.Id == id);
            if (file == null)
            {
                return NotFound();
            }

            var contentType = string.IsNullOrWhiteSpace(file.FileType) ? "application/octet-stream" : file.FileType;
            var downloadName = string.IsNullOrWhiteSpace(file.FileName) ? $"file-{id}" : file.FileName;

            return File(file.FileData, contentType, downloadName);
        }
    }

}
