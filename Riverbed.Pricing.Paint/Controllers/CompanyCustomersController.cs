using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;

namespace Riverbed.Pricing.Paint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyCustomersController : ControllerBase
    {
        private readonly PricingDbContext _context;
        private readonly ILogger<CompanyCustomersController> Logger;

        public CompanyCustomersController(PricingDbContext context, ILogger<CompanyCustomersController> logger)
        {
            _context = context;
            Logger = logger;
        }

        // GET: api/CompanyCustomers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyCustomer>>> GetCompanyCustomers()
        {
            return await _context.CompanyCustomers.ToListAsync();
        }

        [HttpGet("CompanyGuid/{companyGuidId}")]
        public async Task<ActionResult<IEnumerable<CompanyCustomer>>> GetAllCompanyCustomersByGuidAsync(string companyGuidId)
        {
            try
            {
                var customers = await _context.CompanyCustomers.Where(x => x.CompanyId == Guid.Parse(companyGuidId))
                    .OrderBy(c => c.LastName)
                    .ToListAsync();
                return customers;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }  
        }

        // GET: api/CompanyCustomers/5
        [HttpGet("CustomerGuid/{customerId}")]
        public async Task<ActionResult<CompanyCustomer>> GetCompanyCustomerByGuid(string customerId)
        {
            var companyCustomer = await _context.CompanyCustomers.Where(x => x.CustomerId == Guid.Parse(customerId)).FirstOrDefaultAsync(); 

            if (companyCustomer == null)
            {
                return NotFound();
            }

            return companyCustomer;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyCustomer>> GetCompanyCustomer(int id)
        {
            var companyCustomer = await _context.CompanyCustomers.FindAsync(id);

            if (companyCustomer == null)
            {
                return NotFound();
            }

            return companyCustomer;
        }

        // PUT: api/CompanyCustomers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompanyCustomer(int id, CompanyCustomer companyCustomer)
        {
            if (id != companyCustomer.Id)
            {
                return BadRequest();
            }

            _context.Entry(companyCustomer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyCustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CompanyCustomers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CompanyCustomer>> PostCompanyCustomer(CompanyCustomer companyCustomer)
        {
            companyCustomer.CustomerId = Guid.NewGuid();
            companyCustomer.CreatedDate = DateTime.Now;
            companyCustomer.Address1 = companyCustomer.Address1 ?? string.Empty;
            companyCustomer.Address2 = companyCustomer.Address2 ?? string.Empty;
            companyCustomer.City = companyCustomer.City ?? string.Empty;
            companyCustomer.StateCode = companyCustomer.StateCode ?? string.Empty;
            companyCustomer.ZipCode = companyCustomer.ZipCode ?? string.Empty;
            companyCustomer.SecondaryCcEmail = companyCustomer.SecondaryCcEmail ?? string.Empty;
            companyCustomer.SecondaryPhoneNumber = companyCustomer.SecondaryPhoneNumber ?? string.Empty;
            _context.CompanyCustomers.Add(companyCustomer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompanyCustomer", new { id = companyCustomer.Id, customerGuidId = companyCustomer.CustomerId }, companyCustomer);
        }

        // DELETE: api/CompanyCustomers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanyCustomer(int id)
        {
            var companyCustomer = await _context.CompanyCustomers
                .Include(cc => cc.CustomerProjects)
                .FirstOrDefaultAsync(cc => cc.Id == id);

            if (companyCustomer == null)
            {
                return NotFound();
            }

            // Remove all projects and their related data
            if (companyCustomer.CustomerProjects != null)
            {
                foreach (var project in companyCustomer.CustomerProjects)
                {
                    // Get all rooms for this project
                    var rooms = await _context.Rooms
                        .Where(r => r.ProjectDataId == project.Id)
                        .ToListAsync();

                    foreach (var room in rooms)
                    {
                        // Remove all PaintableItems for this room
                        var paintableItems = await _context.PaintableItems
                            .Where(pi => pi.RoomId == room.Id)
                            .ToListAsync();

                        _context.PaintableItems.RemoveRange(paintableItems);
                    }

                    // Remove all rooms for this project
                    _context.Rooms.RemoveRange(rooms);
                }

                // Remove all projects for this customer
                _context.Projects.RemoveRange(companyCustomer.CustomerProjects);
            }

            // Remove the customer
            _context.CompanyCustomers.Remove(companyCustomer);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/CompanyCustomers/delete-with-related/{customerGuid}
        [HttpDelete("delete-with-related/{customerGuid}")]
        public async Task<IActionResult> DeleteCompanyCustomerAndRelatedData(Guid customerGuid)
        {
            var companyCustomer = await _context.CompanyCustomers
                .Include(cc => cc.CustomerProjects)
                .FirstOrDefaultAsync(cc => cc.CustomerId == customerGuid);

            if (companyCustomer == null)
            {
                return NotFound();
            }

            // Remove all projects and their related data
            if (companyCustomer.CustomerProjects != null)
            {
                foreach (var project in companyCustomer.CustomerProjects)
                {
                    // Get all rooms for this project
                    var rooms = await _context.Rooms
                        .Where(r => r.ProjectDataId == project.Id)
                        .ToListAsync();

                    foreach (var room in rooms)
                    {
                        // Remove all PaintableItems for this room
                        var paintableItems = await _context.PaintableItems
                            .Where(pi => pi.RoomId == room.Id)
                            .ToListAsync();
                        _context.PaintableItems.RemoveRange(paintableItems);
                    }

                    // Remove all rooms for this project
                    _context.Rooms.RemoveRange(rooms);
                }

                // Remove all projects for this customer
                _context.Projects.RemoveRange(companyCustomer.CustomerProjects);
            }

            // Remove the customer
            _context.CompanyCustomers.Remove(companyCustomer);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CompanyCustomerExists(int id)
        {
            return _context.CompanyCustomers.Any(e => e.Id == id);
        }
    }
}
