using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;

namespace Riverbed.Pricing.Paint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    /**
     * Controller for managing company paintable items.
     * This controller provides CRUD operations for company paintable items.
     * The CompanyPaintableItem entity is used to represent a paintable item that a company can paint
     * and is used to store information about the item, such as its name, description, price, paint type, pricing type.
     * It is used in dropdowns to populate the PaintableItem field in the Room entity.
     */
    public class CompanyPaintableItemsController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public CompanyPaintableItemsController(PricingDbContext context)
        {
            _context = context;
        }

        // GET: api/CompanyPaintableItems
        [HttpGet("all/{companyGuid}")]
        public async Task<ActionResult<IEnumerable<CompanyPaintableItem>>> GetCompanyPaintableItems(string companyGuid)
        {
            var compId = Guid.Parse(companyGuid);
            return await _context.CompanyPaintableItems
                .Where(pi => pi.CompanyId == compId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the summaries of all company paintable items.
        /// </summary>
        /// <returns>A list of company paintable item summaries.</returns>
        [HttpGet]
        [Route("summary/{companyGuid}")]
        public async Task<ActionResult<IEnumerable<CompanyPaintableItemSummary>>> GetCompanyPaintableItemSummaries(string companyGuid)
        {
             var compId = Guid.Parse(companyGuid);
            var CompanyPaintableItems = await _context.CompanyPaintableItems.Where(c => c.CompanyId == compId).OrderBy(c => c.PaintableItemCategoryId).ToListAsync();
            var CompanyPaintableItemSummaries = new List<CompanyPaintableItemSummary>();
            foreach (var item in CompanyPaintableItems)
            {
                CompanyPaintableItemSummaries.Add(new CompanyPaintableItemSummary
                {
                    Id = item.Id,
                    CompanyPaintableItemId = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    PaintableItemCategoryId = item.PaintableItemCategoryId
                });
            }
            return CompanyPaintableItemSummaries;
        }

        // GET: api/CompanyPaintableItem/5
        [HttpGet("{id}/{companyGuid}")]
        public async Task<ActionResult<CompanyPaintableItem>> GetCompanyPaintableItem(int id, string companyGuid)
        {
            var companyPaintableItem = await _context.CompanyPaintableItems.FindAsync(id);

            if (companyPaintableItem == null)
            {
                return NotFound();
            }

            return companyPaintableItem;
        }

        // PUT: api/CompanyPaintableItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}/{companyGuid}")]
        public async Task<IActionResult> PutCompanyPaintableItem(int id, string companyGuid, CompanyPaintableItem companyPaintableItem)
        {
            if (id != companyPaintableItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(companyPaintableItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyPaintableItemExists(id))
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

        // POST: api/CompanyPaintableItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CompanyPaintableItem>> PostCompanyPaintableItem(CompanyPaintableItem companyPaintableItem)
        {
            try
            {
                _context.CompanyPaintableItems.Add(companyPaintableItem);
                await _context.SaveChangesAsync();

                return CreatedAtAction("PostCompanyPaintableItem", new { id = companyPaintableItem.Id }, companyPaintableItem);
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/CompanyPaintableItems/5
        [HttpDelete("{id}/{companyguid}")]
        public async Task<IActionResult> DeleteCompanyPaintableItem(int id, string companyguid)
        {
            var compId = Guid.Parse(companyguid);
            var companyPaintableItem = await _context.CompanyPaintableItems
                .Where(pi => pi.CompanyId == compId && pi.Id == id)
                .FirstOrDefaultAsync();

            if (companyPaintableItem == null)
            {
                return NotFound();
            }

            _context.CompanyPaintableItems.Remove(companyPaintableItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CompanyPaintableItemExists(int id)
        {
            return _context.CompanyPaintableItems.Any(e => e.Id == id);
        }
    }
}
