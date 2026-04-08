using Microsoft.AspNetCore.Mvc;
using Riverbed.Pricing.Paint.Shared.EmailService;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace Riverbed.Pricing.Paint.Controllers.Utils
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailServiceController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly PricingDbContext _db;

        public EmailServiceController(PricingDbContext db, IEmailService emailService)
        {
            _emailService = emailService;
            _db = db;
        }

        [HttpPost("Send")]
        public async Task<IActionResult> Send([FromBody] EmailRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ToAddress) || string.IsNullOrWhiteSpace(request.Subject) || string.IsNullOrWhiteSpace(request.Body))
            {
                return BadRequest("Missing required fields.");
            }

            // Replace all body Tokens with Project, Customer or Company values


            var result = _emailService.SendEmail(request.ToAddress, request.Subject, request.Body, request.senderName, request.senderEmail, request.CcEmail, request.BccEmail, request.IsHtml);
            if (result.StartsWith("Error"))
                return StatusCode(500, result);

            try
            {
                var fromEmail = string.IsNullOrWhiteSpace(request.senderEmail) ? "quotes@riverbed-software.com" : request.senderEmail;
                Guid? companyGuid = null, projectGuid = null;
                if (Guid.TryParse(request.CompanyGuid, out var cg)) companyGuid = cg;
                if (Guid.TryParse(request.ProjectGuid, out var pg)) projectGuid = pg;

                var log = new EmailLog
                {
                    TimeSent = DateTime.UtcNow,
                    Subject = request.Subject ?? string.Empty,
                    EmailBody = request.Body ?? string.Empty,
                    CompanyReportTypeId = request.CompanyReportTypeId,
                    ProjectGuid = projectGuid,
                    CompanyGuid = companyGuid,
                    ToEmail = request.ToAddress ?? string.Empty,
                    CcEmail = request.CcEmail,
                    FromEmail = fromEmail
                };
                _db.EmailLogs.Add(log);
                await _db.SaveChangesAsync();
            }
            catch { }

            return Ok(result);
        }

        // Alternate endpoint: project-aware send. Uses projectGuid to resolve customer and company guid prior to logging.
        [HttpPost("SendProject/{projectGuid}")]         
        public async Task<IActionResult> SendProject(string projectGuid, [FromBody] EmailRequest request)
        {
            if (string.IsNullOrWhiteSpace(projectGuid))
                return BadRequest("Project guid is required.");
            if (string.IsNullOrWhiteSpace(request.Subject) || string.IsNullOrWhiteSpace(request.Body))
                return BadRequest("Subject and Body are required.");

            if (!Guid.TryParse(projectGuid, out var projectGuidVal))
                return BadRequest("Invalid project guid.");

            // Resolve project, customer and company
            var project = await _db.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.ProjectGuid == projectGuidVal);
            if (project == null)
                return NotFound("Project not found.");

            var customer = await _db.CompanyCustomers.AsNoTracking().FirstOrDefaultAsync(c => c.CustomerId == project.CompanyCustomerGuidId);
            if (customer == null)
                return NotFound("Customer not found for project.");

            var resolvedCompanyGuid = customer.CompanyId;

            // Use customer's email as default ToAddress if not provided
            var toAddress = string.IsNullOrWhiteSpace(request.ToAddress) ? (project.CustomerEmail ?? string.Empty) : request.ToAddress;
            if (string.IsNullOrWhiteSpace(toAddress))
                return BadRequest("ToAddress is required or must be resolvable from project/customer.");

            var result = _emailService.SendEmail(toAddress, request.Subject, request.Body, request.senderName, request.senderEmail, request.CcEmail, request.BccEmail, request.IsHtml);
            if (result.StartsWith("Error"))
                return StatusCode(500, result);

            // Log with resolved guids
            try
            {
                var fromEmail = string.IsNullOrWhiteSpace(request.senderEmail) ? "quotes@riverbed-software.com" : request.senderEmail;
                var log = new EmailLog
                {
                    TimeSent = DateTime.UtcNow,
                    Subject = request.Subject ?? string.Empty,
                    EmailBody = request.Body ?? string.Empty,
                    CompanyReportTypeId = request.CompanyReportTypeId,
                    ProjectGuid = projectGuidVal,
                    CompanyGuid = resolvedCompanyGuid,
                    ToEmail = toAddress,
                    CcEmail = request.CcEmail,
                    BccEmail = request.BccEmail,
                    FromEmail = fromEmail
                };
                _db.EmailLogs.Add(log);
                await _db.SaveChangesAsync();
            }
            catch { }

            return Ok(result);
        }

        // New endpoint: get email logs by project guid -> returns minimal DTO with EmailTypeName
        [HttpGet("Project/{projectGuid}/logs")]
        public async Task<IActionResult> GetProjectEmailLogs(string projectGuid)
        {
            if (!Guid.TryParse(projectGuid, out var guid))
                return BadRequest("Invalid project guid.");

            var logs = await _db.EmailLogs
                .AsNoTracking()
                .Where(e => e.ProjectGuid == guid)
                .OrderByDescending(e => e.TimeSent)
                .Select(e => new EmailLogDto
                {
                    Id = e.Id,
                    TimeSent = e.TimeSent,
                    Subject = e.Subject,
                    ToEmail = e.ToEmail,
                    CcEmail = e.CcEmail,                    
                    EmailTypeName = e.CompanyReportTypeId.HasValue
                        ? _db.CompanyReportTypes.Where(rt => rt.Id == e.CompanyReportTypeId.Value).Select(rt => rt.ReportTypeName).FirstOrDefault()
                        : null
                })
                .ToListAsync();

            return Ok(logs);
        }
    }

    public class EmailRequest
    {
        public string ToAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string senderName { get; set; }
        public string senderEmail { get; set; }
        public string CcEmail { get; set; }
        public string BccEmail { get; set; }
        public bool IsHtml { get; set; } = true;

        // Optional logging context
        public int? CompanyReportTypeId { get; set; }
        public string? ProjectGuid { get; set; }
        public string? CompanyGuid { get; set; }
    }

    public class EmailLogDto
    {
        public int Id { get; set; }
        public DateTime TimeSent { get; set; }
        public string Subject { get; set; }
        public string ToEmail { get; set; }
        public string CcEmail { get; set; }
        public string EmailTypeName { get; set; }
    }
}
