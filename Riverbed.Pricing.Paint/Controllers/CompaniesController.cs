using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Controllers.Utils;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;

namespace Riverbed.Pricing.Paint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly PricingDbContext _context;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(PricingDbContext context, ILogger<CompaniesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Companies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            _logger.LogInformation("Getting all companies");
            return await _context.Companies.ToListAsync();
        }

        // GET: api/Companies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(Guid id)
        {
            _logger.LogInformation($"Getting company with ID: {id}");
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                _logger.LogWarning($"Company with ID: {id} not found");
                return NotFound();
            }

            return company;
        }

        // NEW: minimal projects for a company
        // GET: api/Companies/{companyGuid}/projects/minimal
        [HttpGet("{companyGuid}/projects/minimal")]
        public async Task<ActionResult<IEnumerable<ProjectDataMinimal>>> GetCompanyProjectsMinimal(string companyGuid)
        {
            if (!Guid.TryParse(companyGuid, out var companyId))
            {
                return BadRequest("Invalid company guid.");
            }

            var fromDate = DateTime.UtcNow.AddMonths(-2);

            // Join Projects with CompanyCustomer to get customer name and filter by CompanyId
            var query = from p in _context.Projects
                        join cc in _context.CompanyCustomers on p.CompanyCustomerId equals cc.Id
                        where cc.CompanyId == companyId && p.CreatedDate >= fromDate
                        select new ProjectDataMinimal
                        {
                            ProjectName = p.ProjectName,
                            CustomerName = (cc.FirstName + " " + cc.LastName).Trim(),
                            CreatedDate = p.CreatedDate,
                            ProjectGuid = p.ProjectGuid,
                            StatusCodeId = p.StatusCodeId
                        };

            var list = await query.ToListAsync();

            // Custom order by status: 2, 8, 1, 3, 4, 6, 7, 5
            int OrderKey(int status) => status switch
            {
                2 => 1,
                8 => 2,
                1 => 3,
                3 => 4,
                4 => 5,
                6 => 6,
                7 => 7,
                5 => 8,
                _ => 9
            };

            list = list
                .OrderBy(pm => OrderKey(pm.StatusCodeId))
                .ThenByDescending(pm => pm.CreatedDate)
                .ToList();

            return Ok(list);
        }

        // GET: api/Companies/{companyGuid}/projects/accepted
        [HttpGet("{companyGuid}/projects/accepted")]
        public async Task<ActionResult<IEnumerable<ProjectDataMinimal>>> GetAcceptedProjects(string companyGuid)
        {
            _logger.LogInformation("Getting accepted projects (StatusCodeId = 2) for company {CompanyGuid}", companyGuid);

            if (!Guid.TryParse(companyGuid, out var companyId))
            {
                return BadRequest("Invalid company guid.");
            }

            var query = from p in _context.Projects
                        join cc in _context.CompanyCustomers on p.CompanyCustomerId equals cc.Id
                        where p.StatusCodeId == 2 && cc.CompanyId == companyId
                        select new ProjectDataMinimal
                        {
                            ProjectName = p.ProjectName,
                            CustomerName = (cc.FirstName + " " + cc.LastName).Trim(),
                            CreatedDate = p.CreatedDate,
                            ProjectGuid = p.ProjectGuid,
                            StatusCodeId = p.StatusCodeId
                        };

            var list = await query
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            return Ok(list);
        }

        // PUT: api/Companies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(Guid id, Company company)
        {
            if (id != company.Id)
            {
                _logger.LogWarning($"Company ID mismatch: {id} != {company.Id}");
                return BadRequest();
            }

            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Company with ID: {id} updated successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
                {
                    _logger.LogWarning($"Company with ID: {id} not found during update");
                    return NotFound();
                }
                else
                {
                    _logger.LogError($"Concurrency error while updating company with ID: {id}");
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Companies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Company>> PostCompany(Company company)
        {
            try
            {
                if (company.Id == Guid.Empty)
                    company.Id = Guid.NewGuid();
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Company with ID: {CompanyId} created successfully", company.Id);

                // Provision all default data via the centralized provisioning controller
                var provisioner = HttpContext.RequestServices.GetRequiredService<CompanyProvisioningController>();
                provisioner.ControllerContext = ControllerContext;
                var provisionResult = await provisioner.ProvisionCompanyAsync(company.Id.ToString());

                if (provisionResult is ObjectResult objResult && objResult.StatusCode >= 400)
                {
                    _logger.LogWarning("Provisioning returned status {Status} for company {CompanyId}", objResult.StatusCode, company.Id);
                    // Company was created; provisioning had issues but we don't roll back the company
                }

                return CreatedAtAction("GetCompany", new { id = company.Id }, company);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the company");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the company.");
            }
        }

        // SET: api/Companies/SetDefaults
        // Legacy endpoint — delegates to CompanyProvisioningController
        [HttpGet("SetDefaults/{CompanyGuid}")]
        public async Task<IActionResult> SetDefaults(string CompanyGuid)
        {
            var provisioner = HttpContext.RequestServices.GetRequiredService<CompanyProvisioningController>();
            provisioner.ControllerContext = ControllerContext;
            return await provisioner.ProvisionCompanyAsync(CompanyGuid);
        }

        // DELETE: api/Companies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            _logger.LogInformation($"Deleting company with ID: {id}");
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                _logger.LogWarning($"Company with ID: {id} not found");
                return NotFound();
            }

            _logger.LogInformation($"Deleting All Company Defaults for company with ID: {id}");
            var companyDefaults = new CompanyDefaultUtility(_context, _logger, id);
            await companyDefaults.DeleteCompanyPaintType();
            await companyDefaults.DeleteCompanyPaintableItems();
            await companyDefaults.DeleteCompanyRoomDefaults();
            await companyDefaults.DeleteCompanySettings();
            await companyDefaults.DeleteCompanyInteriorPricing();
            await companyDefaults.DeleteCompanyExteriorPricing();
            await companyDefaults.DeleteServiceTitanDefaults();

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Company with ID: {id} deleted successfully");
            return NoContent();
        }

        // POST: api/Companies/{companyGuid}/CopyGlobalReportTemplates
        [HttpPost("{companyGuid}/CopyGlobalReportTemplates")]
        public async Task<IActionResult> CopyGlobalCompanyHTMLReportTemplates(string companyGuid)
        {
            if (!Guid.TryParse(companyGuid, out var companyId))
            {
                return BadRequest("Invalid company guid.");
            }

            var company = await _context.Companies.FindAsync(companyId);
            if (company == null)
            {
                return NotFound();
            }

            try
            {
                var util = new CompanyDefaultUtility(_context, _logger, companyId);
                await util.CopyGlobalCompanyHTMLReportTemplatesToCompany();
                return Ok("Global report templates copied successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error copying global report templates for company {companyId}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while copying global report templates.");
            }
        }

        // POST: api/Companies/{companyGuid}/SetCompanyInteriorPricing
        [HttpPost("{companyGuid}/SetCompanyInteriorPricing")]
        public async Task<IActionResult> SetCompanyInteriorPricing(string companyGuid)
        {
            if (!Guid.TryParse(companyGuid, out var companyId))
            {
                return BadRequest("Invalid company guid.");
            }

            var company = await _context.Companies.FindAsync(companyId);
            if (company == null)
            {
                return NotFound();
            }

            try
            {
                var util = new CompanyDefaultUtility(_context, _logger, companyId);
                await util.SetCompanyInteriorPricing();
                await _context.SaveChangesAsync();
                return Ok("Company interior pricing set successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error setting company interior pricing for company {companyId}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while setting company interior pricing.");
            }
        }

        private bool CompanyExists(Guid id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }
    }
}
