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
    public class AdjustmentsController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public AdjustmentsController(PricingDbContext context)
        {
            _context = context;
        }

        // GET: api/Adjustments
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Adjustment>>> GetAdjustments()
        //{
        //    return await _context.Adjustments.ToListAsync();
        //}

        // GET: api/Adjustments/5
        [HttpGet("ByProjectId/{id}")]
        public async Task<ActionResult<List<Adjustment>>> GetAdjustments(int projectId)
        {
            var adjustments = _context.Adjustments.Where(a => a.ProjectDataId == projectId);

            if (adjustments == null)
            {
                return NotFound();
            }

            return adjustments.ToList();
        }

        /// <summary>
        /// Updates an existing adjustment.
        /// </summary>
        /// <param name="id">The ID of the adjustment to update.</param>
        /// <param name="adjustment">The adjustment object with updated values.</param>
        /// <returns>No content if the update is successful, or a NotFound result if the adjustment does not exist.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdjustment(int id, Adjustment adjustment)
        {
            adjustment.IsPercentage = adjustment.Type == AdjustmentType.Discount;

            if (adjustment.Id == 0)
            {
                _context.Adjustments.Add(adjustment);
                await _context.SaveChangesAsync();
            }

            _context.Entry(adjustment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!AdjustmentExists(id))
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


        /// <summary>
        /// Creates a new adjustment.
        /// </summary>
        /// <param name="adjustment">The adjustment to create.</param>
        /// <returns>The created adjustment.</returns>
        [HttpPost]
        public async Task<ActionResult<Adjustment>> PostAdjustment(Adjustment adjustment)
        {
            adjustment.IsPercentage = adjustment.Type == AdjustmentType.Discount;
            _context.Adjustments.Add(adjustment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAdjustments), new { id = adjustment.Id }, adjustment);
        }

        // DELETE: api/Adjustments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdjustment(int id)
        {
            var adjustment = await _context.Adjustments.FindAsync(id);
            if (adjustment == null)
            {
                return NotFound();
            }

            _context.Adjustments.Remove(adjustment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdjustmentExists(int id)
        {
            return _context.Adjustments.Any(e => e.Id == id);
        }
    }
}
