using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;

namespace Riverbed.Pricing.Paint.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerProjectsController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public CustomerProjectsController(PricingDbContext context)
        {
            _context = context;
        }

        // GET: api/CustomerProjects
        [HttpGet("all/{customerId}")]
        public async Task<ActionResult<IEnumerable<ProjectData>>> GetAllCustomerProjects(int customerId)
        {
            var customerProjects = await _context.Projects.Where(c => c.CompanyCustomerId == customerId).OrderByDescending(p => p.Id).ToListAsync();
            if (customerProjects == null)
            {
                return new List<ProjectData>();
            }

            // Walk through each project and check for BaseAmount null value or zero
            foreach (var project in customerProjects)
            {
                if (project.BaseAmount <= 0)
                {
                    // Walk through associated Rooms to calculate BaseAmount
                    var rooms = await _context.Rooms.Where(r => r.ProjectDataId == project.Id).ToListAsync();
                    decimal totalBaseAmount = 0;
                    foreach (var room in rooms)
                    {
                        if (room.IsOptional) continue;

                        // Check for nulls and set to zero if necessary
                        if (room.LaborCost == null) room.LaborCost = 0;
                        if (room.MaterialCost == null) room.MaterialCost = 0;
                        if (room.OverheadCost == null) room.OverheadCost = 0;
                        if (room.Profit == null) room.Profit = 0;
                        totalBaseAmount = (decimal)(room.LaborCost + room.MaterialCost + room.OverheadCost + room.Profit);
                        project.BaseAmount += totalBaseAmount;                        
                    }
                }
            }
            _context.SaveChangesAsync();

            return customerProjects;
        }

        
        // GET: api/CustomerProjects/5hkljlkaf-lkjlkajsf-798798fasd
        [HttpGet("{projectId}")]
        public async Task<ActionResult<ProjectData>> GetCustomerProject(string projectId)
        {
            if (projectId == null || projectId == "0")
            {
                return BadRequest();
            }

            var customerProject = await _context.Projects.Where(p => p.ProjectGuid == Guid.Parse(projectId)).FirstOrDefaultAsync();

            if (customerProject == null)
            {
                return NotFound();
            }
            var adjustments = await _context.Adjustments.Where(a => a.ProjectDataId == customerProject.Id).ToListAsync();
            customerProject.Adjustments = adjustments ?? new List<Adjustment>();

            return customerProject;
        }

        // PUT: api/CustomerProjects/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update/{id}")]
        public async Task<IActionResult> PutCustomerProject(int id, ProjectData customerProject)
        {
            if (id != customerProject.Id)
            {
                return BadRequest();
            }

            // Normalize optional fields to prevent validation errors on empty strings
            customerProject.NormalizeOptionalFields();

            _context.Entry(customerProject).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerProjectExists(id))
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

        // POST: api/CustomerProjects
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProjectData>> PostCustomerProject(ProjectData customerProject)
        {
            try
            {
                var companyCustomer = await _context.CompanyCustomers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cc => cc.Id == customerProject.CompanyCustomerId);

                customerProject.ProjectGuid = Guid.NewGuid();
                if (companyCustomer != null)
                {
                    customerProject.SecondaryCcEmail = companyCustomer.SecondaryCcEmail ?? customerProject.SecondaryCcEmail;
                    customerProject.SecondaryPhoneNumber = companyCustomer.SecondaryPhoneNumber ?? customerProject.SecondaryPhoneNumber;
                }

                // Normalize optional fields to prevent validation errors on empty strings
                customerProject.NormalizeOptionalFields();

                _context.Projects.Add(customerProject);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return CreatedAtAction("GetCustomerProject", new { projectId = customerProject.Id }, customerProject);
        }

        // POST: api/CustomerProjects/duplicate/{projectGuid}
        [HttpPost("duplicate/{projectGuid}")]
        public async Task<ActionResult<ProjectData>> DuplicateCustomerProject(string projectGuid, [FromBody] ProjectData newProjectData)
        {
            if (string.IsNullOrEmpty(projectGuid) || !Guid.TryParse(projectGuid, out var sourceGuid))
            {
                return BadRequest("Invalid project guid.");
            }

            var sourceProject = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectGuid == sourceGuid);
            if (sourceProject == null)
            {
                return NotFound("Source project not found.");
            }

            // Create a new project using the provided newProjectData
            var companyCustomer = await _context.CompanyCustomers
                .AsNoTracking()
                .FirstOrDefaultAsync(cc => cc.Id == newProjectData.CompanyCustomerId);

            var newProject = new ProjectData
            {
                ProjectGuid = Guid.NewGuid(),
                ProjectName = newProjectData.ProjectName,
                CustomerPhone = newProjectData.CustomerPhone,
                CustomerEmail = newProjectData.CustomerEmail,
                SecondaryCcEmail = newProjectData.SecondaryCcEmail ?? companyCustomer?.SecondaryCcEmail,
                SecondaryPhoneNumber = newProjectData.SecondaryPhoneNumber ?? companyCustomer?.SecondaryPhoneNumber,
                Address1 = newProjectData.Address1,
                Address2 = newProjectData.Address2,
                City = newProjectData.City,
                StateCode = newProjectData.StateCode,
                ZipCode = newProjectData.ZipCode,
                StatusCodeId = 1,
                CompanyCustomerId = newProjectData.CompanyCustomerId,
                CompanyCustomerGuidId = newProjectData.CompanyCustomerGuidId,
                BaseAmount = sourceProject.BaseAmount,
                Summary = sourceProject.Summary,
                ScopeOfWork = sourceProject.ScopeOfWork,
                CreatedDate = DateTime.UtcNow,
                CompletedDate = DateTime.UtcNow.AddMonths(6)
            };

            // Normalize optional fields to prevent validation errors on empty strings
            newProject.NormalizeOptionalFields();

            _context.Projects.Add(newProject);
            await _context.SaveChangesAsync();

            // Copy associated Rooms from the source project
            var sourceRooms = await _context.Rooms
                .Include(r => r.PaintableItems)
                .Where(r => r.ProjectDataId == sourceProject.Id)
                .ToListAsync();
            foreach (var room in sourceRooms)
            {
                var newRoom = new Room
                {
                    PaintQualityId = room.PaintQualityId,
                    ProjectDataId = newProject.Id,
                    ProjectGuid = newProject.ProjectGuid,
                    Name = room.Name,
                    Notes = room.Notes.Length == 0 ? "" : room.Notes,
                    IsChangeOrder = false,
                    Length = room.Length,
                    Width = room.Width,
                    Height = room.Height,
                    IsOptional = false,
                    PrimeWalls = room.PrimeWalls,
                    NumberOfDoors = room.NumberOfDoors,
                    NumberOfWindows = room.NumberOfWindows,
                    IncludeCeilings = room.IncludeCeilings,
                    IncludeBaseboards = room.IncludeBaseboards,
                    IncludeCrownMoldings = room.IncludeCrownMoldings,
                    IncludeWalls = room.IncludeWalls,
                    AdditionalPrepTime = room.AdditionalPrepTime,
                    AreaTotal = room.AreaTotal,
                    LaborCost = room.LaborCost,
                    MaterialCost = room.MaterialCost,
                    OverheadCost = room.OverheadCost,
                    Profit = room.Profit,                    
                };
                _context.Rooms.Add(newRoom);
                await _context.SaveChangesAsync();

                // Walk through Paintable Items and copy them to the new room
                foreach (var paintableItem in room.PaintableItems)
                {
                    var newPaintableItem = new PaintableItem
                    {
                        Name = paintableItem.Name,
                        Description = paintableItem.Description,
                        Price = paintableItem.Price,
                        PaintTypeId = paintableItem.PaintTypeId,
                        PricingTypeId = paintableItem.PricingTypeId,
                        SquareFootage = paintableItem.SquareFootage,
                        BaseTime = paintableItem.BaseTime,
                        AdditionalTime = paintableItem.AdditionalTime,
                        Count = paintableItem.Count,    
                        RoomId = newRoom.Id,
                        Coats = paintableItem.Coats,
                        PaintableItemCategoryId = paintableItem.PaintableItemCategoryId, 
                    };
                    newRoom.PaintableItems.Add(newPaintableItem);
                    await _context.SaveChangesAsync();
                }
            }
            
            return CreatedAtAction("GetCustomerProject", new { projectId = newProject.ProjectGuid }, newProject);
        }

        // DELETE: api/CustomerProjects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerProject(int id)
        {
            var customerProject = await _context.Projects.FindAsync(id);
            if (customerProject == null)
            {
                return NotFound();
            }            

            // Delete all associated data
            var rooms = _context.Rooms.Where(pi => pi.ProjectDataId == id).ToList();
            foreach (var room in rooms)
            {
                // Delete all Paintable Items associated with the project
                var paintableItems = _context.PaintableItems.Where(pi => pi.RoomId == room.Id).ToList();
                foreach (var paintableItem in paintableItems)
                {
                    _context.PaintableItems.Remove(paintableItem);
                }
                _context.Rooms.Remove(room);
            }
            await _context.SaveChangesAsync();

            // Now delete the project
            _context.Projects.Remove(customerProject);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // New endpoint to update BaseAmount
        [HttpPatch("{projectId}/base-amount")]
        public async Task<IActionResult> UpdateBaseAmount(int projectId, [FromBody] decimal baseAmount)
        {
            try
            {
                // Find the project by ID
                var project = await _context.Projects.FindAsync(projectId);
                if (project == null)
                {
                    return NotFound(new { Message = $"Project with ID {projectId} not found." });
                }

                // Update the BaseAmount
                project.BaseAmount = baseAmount;

                // Save changes to the database
                await _context.SaveChangesAsync();

                return Ok(new { Message = "BaseAmount updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating the BaseAmount.", Error = ex.Message });
            }
        }

        private bool CustomerProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
