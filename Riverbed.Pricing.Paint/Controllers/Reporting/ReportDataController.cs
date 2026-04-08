using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Controllers.Reporting
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportDataController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public ReportDataController(PricingDbContext context)
        {
            _context = context;
        }
         
        [HttpGet("GetAllCompanyProjects")]
        public async Task<ActionResult<List<CompanyProjectsDto>>> GetAllCompanyProjects()
        {
            var query =
                from c in _context.Companies
                join cc in _context.CompanyCustomers on c.Id equals cc.CompanyId into customerGroup
                from cc in customerGroup.DefaultIfEmpty()
                join p in _context.Projects on cc.Id equals p.CompanyCustomerId into projectGroup
                from p in projectGroup.DefaultIfEmpty()
                where p != null
                    && p.StatusCodeId != null
                    && p.ProjectName != null
                    && p.CreatedDate != null
                    && p.CompletedDate != null
                    && p.Id != null
                orderby
                    c.CompanyName,
                    (cc.LastName ?? string.Empty),
                    (cc.FirstName ?? string.Empty),
                    p.CreatedDate
                select new
                {
                    c.CompanyName,
                    CustomerName = cc == null ? null : cc.FirstName + " " + cc.LastName,
                    p.ProjectName,
                    p.StatusCodeId,
                    p.CreatedDate,
                    p.CompletedDate,
                    ProjectId = (int?)p.Id
                };

            var data = await query.AsNoTracking().ToListAsync();

            // Get all relevant project ids
            var projectIds = data.Where(x => x.ProjectId.HasValue).Select(x => x.ProjectId.Value).Distinct().ToList();

            // Get all rooms for these projects
            var rooms = await _context.Rooms
                .Include(r => r.PaintableItems)
                .Where(r => projectIds.Contains(r.ProjectDataId))
                .ToListAsync();

            // Group rooms by project id and sum costs
            var projectRoomSums = rooms
                .GroupBy(r => r.ProjectDataId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(r => (decimal)(r.LaborCost ?? 0) + (decimal)(r.MaterialCost ?? 0) + (decimal)(r.OverheadCost ?? 0))
                );

            var result = data.Select(x => new CompanyProjectsDto(
                x.CompanyName,
                x.CustomerName,
                x.ProjectName,
                x.StatusCodeId,
                x.CreatedDate,
                x.CompletedDate,
                x.ProjectId != null && projectRoomSums.ContainsKey(x.ProjectId.Value) ? projectRoomSums[x.ProjectId.Value] : 0
            )).ToList();

            return Ok(result);
        }
    }
}
