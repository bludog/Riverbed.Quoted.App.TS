using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Entities;
using Riverbed.Pricing.Paint.Shared.Data;

namespace Riverbed.Pricing.Paint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceTitanConnectionDatasController : ControllerBase
    {
        private readonly PricingDbContext _context;
        private readonly ILogger<ServiceTitanConnectionDatasController> _logger;

        public ServiceTitanConnectionDatasController(PricingDbContext context, ILogger<ServiceTitanConnectionDatasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/ServiceTitanConnectionDatas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceTitanConnectionData>>> GetServiceTitanConnectionDatas()
        {
            _logger.LogInformation("Getting all ServiceTitanConnectionDatas");
            return await _context.ServiceTitanConnectionDatas.ToListAsync();
        }

        // GET: api/ServiceTitanConnectionDatas/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ServiceTitanConnectionData>> GetServiceTitanConnectionData(int id)
        {
            _logger.LogInformation($"Getting ServiceTitanConnectionData with id: {id}");
            var serviceTitanConnectionData = await _context.ServiceTitanConnectionDatas.FindAsync(id);

            if (serviceTitanConnectionData == null)
            {
                _logger.LogWarning($"ServiceTitanConnectionData with id: {id} not found");
                return NotFound();
            }

            return serviceTitanConnectionData;
        }

        // GET: api/ServiceTitanConnectionDatas/company/{companyGuid}
        [HttpGet("Company/{companyGuid}")]
        public async Task<ActionResult<ServiceTitanConnectionData>> GetServiceTitanConnectionDataByCompanyGuid(string companyGuid)
        {
            _logger.LogInformation($"Getting ServiceTitanConnectionData with company guid: {companyGuid}");

            if (string.IsNullOrEmpty(companyGuid))
            {
                _logger.LogWarning("Company GUID was null or empty");
                return BadRequest("Company GUID cannot be null or empty");
            }

            if (!Guid.TryParse(companyGuid, out Guid parsedGuid))
            {
                _logger.LogWarning($"Invalid GUID format: {companyGuid}");
                return BadRequest("Invalid GUID format");
            }

            var serviceTitanConnectionData = await _context.ServiceTitanConnectionDatas
                .FirstOrDefaultAsync(c => c.CompanyGuid == parsedGuid);

            if (serviceTitanConnectionData == null)
            {
                _logger.LogWarning($"ServiceTitanConnectionData with company guid: {companyGuid} not found");
                return NotFound();
            }

            return serviceTitanConnectionData;
        }

        // PUT: api/ServiceTitanConnectionDatas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServiceTitanConnectionData(int id, ServiceTitanConnectionData serviceTitanConnectionData)
        {
            _logger.LogInformation($"Updating ServiceTitanConnectionData with id: {id}");

            if (id != serviceTitanConnectionData.Id)
            {
                _logger.LogWarning($"ID mismatch: URL id: {id}, Object id: {serviceTitanConnectionData.Id}");
                return BadRequest("ID mismatch");
            }

            _context.Entry(serviceTitanConnectionData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"ServiceTitanConnectionData with id: {id} updated successfully");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ServiceTitanConnectionDataExists(id))
                {
                    _logger.LogWarning($"ServiceTitanConnectionData with id: {id} not found during update");
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, $"Error updating ServiceTitanConnectionData with id: {id}");
                    throw;
                }
            }

            return Ok(serviceTitanConnectionData);
        }

        // POST: api/ServiceTitanConnectionDatas
        [HttpPost]
        public async Task<ActionResult<ServiceTitanConnectionData>> PostServiceTitanConnectionData(ServiceTitanConnectionData serviceTitanConnectionData)
        {
            _logger.LogInformation("Creating new ServiceTitanConnectionData");
            
            try
            {
                _context.ServiceTitanConnectionDatas.Add(serviceTitanConnectionData);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"ServiceTitanConnectionData created with id: {serviceTitanConnectionData.Id}");

                return CreatedAtAction("GetServiceTitanConnectionData", new { id = serviceTitanConnectionData.Id }, serviceTitanConnectionData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ServiceTitanConnectionData");
                return StatusCode(500, "An error occurred while creating the ServiceTitanConnectionData");
            }
        }

        // DELETE: api/ServiceTitanConnectionDatas/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceTitanConnectionData>> DeleteServiceTitanConnectionData(int id)
        {
            _logger.LogInformation($"Deleting ServiceTitanConnectionData with id: {id}");

            var serviceTitanConnectionData = await _context.ServiceTitanConnectionDatas.FindAsync(id);
            if (serviceTitanConnectionData == null)
            {
                _logger.LogWarning($"ServiceTitanConnectionData with id: {id} not found for deletion");
                return NotFound();
            }

            try
            {
                _context.ServiceTitanConnectionDatas.Remove(serviceTitanConnectionData);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"ServiceTitanConnectionData with id: {id} deleted successfully");

                return serviceTitanConnectionData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting ServiceTitanConnectionData with id: {id}");
                throw;
            }
        }

        private bool ServiceTitanConnectionDataExists(int id)
        {
            return _context.ServiceTitanConnectionDatas.Any(e => e.Id == id);
        }
    }
}