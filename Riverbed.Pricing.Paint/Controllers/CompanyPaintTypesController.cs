using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;

namespace Riverbed.Pricing.Paint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyPaintTypesController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public CompanyPaintTypesController(PricingDbContext context)
        {
            _context = context;
        }


        // GET: api/CompanyPaintTypes/b3e1a8d2-4f5b-4c3a-8b2d-1a2e3f4b5c6d
        [HttpGet("{companyGuid}")]
        public async Task<IActionResult> GetCompanyPaintTypes(string companyGuid)
        {
            try
            {
                //var paintTypes = await _context.CompanyPaintTypes.Where(c => c.CompanyId == companyGuid).ToListAsync();
                var paintTypes = await _context.CompanyPaintTypes
                    .Include(cpt => cpt.PaintBrand)
                    .Include(cpt => cpt.PaintSheen)
                    .Where(cpt => cpt.CompanyId.ToString() == companyGuid)
                    .ToListAsync();
                return Ok(paintTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/PaintTypes/5/b3e1a8d2-4f5b-4c3a-8b2d-1a2e3f4b5c6d
        [HttpGet("{id}/{companyGuid}")]
        public async Task<ActionResult<CompanyPaintType>> GetPaintType(int id, string companyGuid)
        {
            var paintTypes = await _context.CompanyPaintTypes
                .Where(pt => pt.Id == id && pt.CompanyId.ToString() == companyGuid)
                .FirstOrDefaultAsync();

            if (paintTypes == null)
            {
                return NotFound();
            }

            return paintTypes;
        }

        // PUT: api/PaintTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}/{companyGuid}")]
        public async Task<IActionResult> PutPaintType(int id, string companyGuid, CompanyPaintType paintType)
        {
            if (id != paintType.Id || companyGuid != paintType.CompanyId.ToString())
            {
                return BadRequest();
            }

            _context.Entry(paintType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaintTypeExists(id, companyGuid))
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

        // POST: api/PaintTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CompanyPaintType>> PostCompanyPaintType(CompanyPaintType paintType)
        {
            try
            {
                _context.CompanyPaintTypes.Add(paintType);
                await _context.SaveChangesAsync();
                return CreatedAtAction("PostCompanyPaintType", new { id = paintType.Id }, paintType);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status412PreconditionFailed, ex.Message);
            }
        }

        // DELETE: api/PaintTypes/5
        [HttpDelete("{id}/{companyGuid}")]
        public async Task<IActionResult> DeleteCompanyPaintType(int id, string companyGuid)
        {
            var paintType = await _context.CompanyPaintTypes
                .Where(pt => pt.Id == id && pt.CompanyId.ToString() == companyGuid)
                .FirstOrDefaultAsync();
            if (paintType == null)
            {
                return NotFound();
            }

            _context.CompanyPaintTypes.Remove(paintType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PaintTypeExists(int id, string companyGuid)
        {
            return _context.CompanyPaintTypes.Any(e => e.Id == id && e.CompanyId.ToString() == companyGuid);
        }

        //// GET: CompanyPaintTypes/b3e1a8d2-4f5b-4c3a-8b2d-1a2e3f4b5c6d
        //public async Task<IActionResult> Index(Guid companyId)
        //{
        //    var pricingDbContext = _context.CompanyPaintTypes.Include(c => c.PaintBrand).Include(c => c.PaintSheen);
        //    return View(await pricingDbContext.ToListAsync());
        //}

        //// GET: CompanyPaintTypes/Details/5/b3e1a8d2-4f5b-4c3a-8b2d-1a2e3f4b5c6d
        //public async Task<IActionResult> Details(int? id, Guid? companyId)
        //{
        //    if (id == null || companyId == null)
        //    {
        //        return NotFound();
        //    }

        //    var companyPaintType = await _context.CompanyPaintTypes
        //        .Include(c => c.PaintBrand)
        //        .Include(c => c.PaintSheen)
        //        .FirstOrDefaultAsync(m => m.Id == id && m.CompanyId == companyId);
        //    if (companyPaintType == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(companyPaintType);
        //}

        //// GET: CompanyPaintTypes/Create
        //public IActionResult Create()
        //{
        //    ViewData["PaintBrandId"] = new SelectList(_context.PaintBrands, "Id", "PaintBrandName");
        //    ViewData["PaintSheenId"] = new SelectList(_context.PaintSheens, "Id", "PaintSheenName");
        //    return View();
        //}

        //// POST: CompanyPaintTypes/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,CompanyId,PaintTypeName,CoverageOneCoatSqFt,CoverageTwoCoatsSqFt,PricePerGallon,PaintSheenId,PaintBrandId")] CompanyPaintType companyPaintType)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(companyPaintType);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["PaintBrandId"] = new SelectList(_context.PaintBrands, "Id", "PaintBrandName", companyPaintType.PaintBrandId);
        //    ViewData["PaintSheenId"] = new SelectList(_context.PaintSheens, "Id", "PaintSheenName", companyPaintType.PaintSheenId);
        //    return View(companyPaintType);
        //}

        //// GET: CompanyPaintTypes/Edit/5/b3e1a8d2-4f5b-4c3a-8b2d-1a2e3f4b5c6d
        //public async Task<IActionResult> Edit(int? id, Guid? companyId)
        //{
        //    if (id == null || companyId == null)
        //    {
        //        return NotFound();
        //    }

        //    var companyPaintType = await _context.CompanyPaintTypes
        //        .Where(cpt => cpt.Id == id && cpt.CompanyId == companyId)
        //        .FirstOrDefaultAsync();
        //    if (companyPaintType == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["PaintBrandId"] = new SelectList(_context.PaintBrands, "Id", "PaintBrandName", companyPaintType.PaintBrandId);
        //    ViewData["PaintSheenId"] = new SelectList(_context.PaintSheens, "Id", "PaintSheenName", companyPaintType.PaintSheenId);
        //    return View(companyPaintType);
        //}

        //// POST: CompanyPaintTypes/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, Guid companyId,[Bind("Id,CompanyId,PaintTypeName,CoverageOneCoatSqFt,CoverageTwoCoatsSqFt,PricePerGallon,PaintSheenId,PaintBrandId")] CompanyPaintType companyPaintType)
        //{
        //    if (id != companyPaintType.Id && companyId == companyPaintType.CompanyId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(companyPaintType);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CompanyPaintTypeExists(companyPaintType.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["PaintBrandId"] = new SelectList(_context.PaintBrands, "Id", "PaintBrandName", companyPaintType.PaintBrandId);
        //    ViewData["PaintSheenId"] = new SelectList(_context.PaintSheens, "Id", "PaintSheenName", companyPaintType.PaintSheenId);
        //    return View(companyPaintType);
        //}

        //// GET: CompanyPaintTypes/Delete/5/b3e1a8d2-4f5b-4c3a-8b2d-1a2e3f4b5c6d
        //public async Task<IActionResult> Delete(int? id, Guid? companyId)
        //{
        //    if (id == null || companyId == null)
        //    {
        //        return NotFound();
        //    }

        //    var companyPaintType = await _context.CompanyPaintTypes
        //        .Include(c => c.PaintBrand)
        //        .Include(c => c.PaintSheen)
        //        .FirstOrDefaultAsync(m => m.Id == id && m.CompanyId == companyId);
        //    if (companyPaintType == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(companyPaintType);
        //}

        //// POST: CompanyPaintTypes/Delete/5/b3e1a8d2-4f5b-4c3a-8b2d-1a2e3f4b5c6d
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id, Guid companyId)
        //{
        //    var companyPaintType = await _context.CompanyPaintTypes
        //        .Where(cpt => cpt.Id == id && cpt.CompanyId == companyId)
        //        .FirstOrDefaultAsync();
        //    if (companyPaintType != null)
        //    {
        //        _context.CompanyPaintTypes.Remove(companyPaintType);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool CompanyPaintTypeExists(int id)
        //{
        //    return _context.CompanyPaintTypes.Any(e => e.Id == id);
        //}
    }
}
