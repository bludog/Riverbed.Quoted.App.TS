using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;

namespace Riverbed.Pricing.Paint.Controllers.Utils
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly PricingDbContext _db;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(PricingDbContext db, ILogger<DashboardController> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// Gets dashboard statistics and recent projects for a company
        /// </summary>
        [HttpGet("{companyGuid}")]
        public async Task<ActionResult<DashboardData>> GetDashboardData(string companyGuid)
        {
            if (string.IsNullOrWhiteSpace(companyGuid) || !Guid.TryParse(companyGuid, out var companyGuidVal))
            {
                return BadRequest("Invalid company guid.");
            }

            try
            {
                var company = await _db.Companies
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == companyGuidVal);

                if (company == null)
                {
                    return NotFound("Company not found.");
                }

                var now = DateTime.Now;
                var startOfYear = new DateTime(now.Year, 1, 1);
                var startOfMonth = new DateTime(now.Year, now.Month, 1);
                var twoMonthsAgo = now.AddMonths(-2);

                // Status codes we care about
                const int statusWon = 2;
                const int statusNew = 1;
                const int statusProposed = 8;
                const int statusCompleted = 6;
                var relevantStatuses = new[] { statusWon, statusNew, statusProposed, statusCompleted };

                // Single query: filter by company through customer relationship and only relevant statuses
                var projects = await _db.Projects
                    .AsNoTracking()
                    .Where(p => p.CompanyCustomer != null 
                        && p.CompanyCustomer.CompanyId == companyGuidVal
                        && relevantStatuses.Contains(p.StatusCodeId))
                    .Select(p => new
                    {
                        p.ProjectName,
                        CustomerName = p.CompanyCustomer != null 
                            ? $"{p.CompanyCustomer.FirstName} {p.CompanyCustomer.LastName}" 
                            : string.Empty,
                        p.CreatedDate,
                        p.ProjectGuid,
                        p.StatusCodeId,
                        p.TotalPrice
                    })
                    .ToListAsync();

                // Year statistics
                var yearProjects = projects.Where(p => p.CreatedDate >= startOfYear).ToList();
                var wonYear = projects.Where(p => p.StatusCodeId == statusWon).ToList(); // All won projects count toward year totals
                var activeYear = yearProjects.Where(p => p.StatusCodeId == statusNew || p.StatusCodeId == statusProposed).ToList();
                var completedYear = yearProjects.Where(p => p.StatusCodeId == statusCompleted).ToList();

                // Month statistics
                var monthProjects = projects.Where(p => p.CreatedDate >= startOfMonth).ToList();
                var wonMonth = monthProjects.Where(p => p.StatusCodeId == statusWon).ToList();
                var activeMonth = monthProjects.Where(p => p.StatusCodeId == statusNew || p.StatusCodeId == statusProposed).ToList();
                var completedMonth = monthProjects.Where(p => p.StatusCodeId == statusCompleted).ToList();

                // Recent project lists (last 2 months) stays for other uses
                var recentProjects = projects.Where(p => p.CreatedDate >= twoMonthsAgo).ToList();
                // All won projects (no time restriction)
                var allWonProjects = projects
                    .Where(p => p.StatusCodeId == statusWon)
                    .OrderByDescending(p => p.CreatedDate)
                    .Select(p => new ProjectDataMinimal
                    {
                        ProjectName = p.ProjectName ?? string.Empty,
                        CustomerName = p.CustomerName,
                        CreatedDate = p.CreatedDate,
                        ProjectGuid = p.ProjectGuid,
                        StatusCodeId = p.StatusCodeId
                    })
                    .ToList();

                var dashboardData = new DashboardData
                {
                    CompanyName = company.CompanyName ?? "Unknown Company",

                    // Year statistics
                    WonQuotesYear = wonYear.Count,
                    WonQuotesTotalYear = wonYear.Sum(p => p.TotalPrice),
                    ActiveQuotesYear = activeYear.Count,
                    ActiveQuotesTotalYear = activeYear.Sum(p => p.TotalPrice),
                    CompletedJobsYear = completedYear.Count,
                    CompletedJobsTotalYear = completedYear.Sum(p => p.TotalPrice),

                    // Month statistics
                    WonQuotesMonth = wonMonth.Count,
                    WonQuotesTotalMonth = wonMonth.Sum(p => p.TotalPrice),
                    ActiveQuotesMonth = activeMonth.Count,
                    ActiveQuotesTotalMonth = activeMonth.Sum(p => p.TotalPrice),
                    CompletedJobsMonth = completedMonth.Count,
                    CompletedJobsTotalMonth = completedMonth.Sum(p => p.TotalPrice),

                    // Project lists
                    WonProjects = allWonProjects,
                    PendingProjects = recentProjects
                        .Where(p => p.StatusCodeId == statusNew || p.StatusCodeId == statusProposed)
                        .OrderByDescending(p => p.CreatedDate)
                        .Select(p => new ProjectDataMinimal
                        {
                            ProjectName = p.ProjectName ?? string.Empty,
                            CustomerName = p.CustomerName,
                            CreatedDate = p.CreatedDate,
                            ProjectGuid = p.ProjectGuid,
                            StatusCodeId = p.StatusCodeId
                        })
                        .ToList()
                };

                _logger.LogInformation("Dashboard data loaded for company {CompanyName}", company.CompanyName);
                return Ok(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data for company {CompanyGuid}", companyGuid);
                return StatusCode(500, "Error loading dashboard data.");
            }
        }
    }
}
