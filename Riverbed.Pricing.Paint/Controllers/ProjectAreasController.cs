using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;
using System.Data.SqlClient;
using Riverbed.Pricing.Paint.Shared.Entities.StoredProc;

namespace Riverbed.Pricing.Paint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectAreasController : ControllerBase
    {
        private readonly PricingDbContext _context;

        public ProjectAreasController(PricingDbContext context)
        {
            _context = context;
        }

        // GET: api/ProjectAreas
        [HttpGet("ByProjectId/{projectId}")]
        public async Task<ActionResult<IEnumerable<ProjectArea>>> GetProjectAreas(int projectId)
        {
            var projectAreas = _context.ProjectAreas.Where(p => p.ProjectDataId == projectId);

            if (projectAreas == null)
            {
                projectAreas = new List<ProjectArea>().AsQueryable();
            }

            return projectAreas.ToList();
        }

        // GET: api/ProjectAreas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectArea>> GetProjectArea(int id)
        {
            var projectArea =  _context.ProjectAreas.Where(p => p.Id == id);

            if (projectArea == null)
            {
                return NotFound();
            }
            var area = projectArea.FirstOrDefault();
            
            return area;
        }

        // Example: https://bludog-software.com/rbp/api/ProjectAreas/ByGuid/7F8B0CA4-7BA7-4B5B-A5BF-57E45ACF2D16
        // Get Pricing data for Project using the GetProjectWithCompanyAndRoomsAndSurfaces stored procedure
        [HttpGet("ByGuid/{projectGuid}")]
        public async Task<ProjectComplexDetails> GetProjectDataByGuid(string projectGuid)
        {
            var projectComplexDetails = new ProjectComplexDetails
            {
                RoomDetails = new List<RoomDetails>()
            };

            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "GetProjectWithCompanyAndRoomsAndSurfaces";
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@CustomerProjectGuid";
                    parameter.Value = projectGuid;
                    command.Parameters.Add(parameter);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        await LoadProjectData(projectComplexDetails, reader);

                        // Read Room Details
                        await LoadAllRoomData(projectComplexDetails, reader);  

                        // Read Company Customer Details
                        await LoadCompanyCustomerData(projectComplexDetails, reader);

                        // Read Company Details
                        await LoadCompanyData(projectComplexDetails, reader);  
                    }
                }
            }

            return projectComplexDetails;
        }

        private static async Task LoadCompanyData(ProjectComplexDetails projectComplexDetails, System.Data.Common.DbDataReader reader)
        {
            if (await reader.NextResultAsync())
            {
                if (reader.HasRows && await reader.ReadAsync())
                {
                    projectComplexDetails.CompanyDetails = new CompanyDetails
                    {
                        CompanyId = reader.GetGuid(reader.GetOrdinal("CompanyId")),
                        CompanyName = reader.GetString(reader.GetOrdinal("CompanyName")),
                        PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                        CompanySettingsId = reader.GetInt32(reader.GetOrdinal("CompanySettingsId")),
                        CompanyDefaultsId = reader.GetInt32(reader.GetOrdinal("CompanyDefaultsId")),
                        Address1 = reader.GetString(reader.GetOrdinal("Address1")),
                        Address2 = reader.IsDBNull(reader.GetOrdinal("Address2")) ? null : reader.GetString(reader.GetOrdinal("Address2")),
                        City = reader.GetString(reader.GetOrdinal("City")),
                        ZipCode = reader.GetString(reader.GetOrdinal("ZipCode")),
                        StateCode = reader.GetString(reader.GetOrdinal("StateCode")),
                        Email = reader.GetString(reader.GetOrdinal("Email"))
                        // TODO: Add StatusCode
                        // TODO: Add TotalCost
                    };
                }
            }
        }

        private static async Task LoadCompanyCustomerData(ProjectComplexDetails projectComplexDetails, System.Data.Common.DbDataReader reader)
        {
            if (await reader.NextResultAsync())
            {
                if (reader.HasRows && await reader.ReadAsync())
                {
                    projectComplexDetails.CompanyCustomerDetails = new CompanyCustomerDetails
                    {
                        CompanyCustomerId = reader.GetInt32(reader.GetOrdinal("CompanyCustomerId")),
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                        CompanyId = reader.GetGuid(reader.GetOrdinal("CompanyId")),
                        CustomerAddress1 = reader.GetString(reader.GetOrdinal("CustomerAddress1")),
                        CustomerAddress2 = reader.IsDBNull(reader.GetOrdinal("CustomerAddress2")) ? null : reader.GetString(reader.GetOrdinal("CustomerAddress2")),
                        CustomerCity = reader.GetString(reader.GetOrdinal("CustomerCity")),
                        CustomerZipCode = reader.GetString(reader.GetOrdinal("CustomerZipCode")),
                        CustomerStateCode = reader.GetString(reader.GetOrdinal("CustomerStateCode")),
                        CustomerEmail = reader.GetString(reader.GetOrdinal("CustomerEmail")),
                        CustomerId = reader.GetGuid(reader.GetOrdinal("CustomerId")),
                        CompanyName = reader.GetString(reader.GetOrdinal("CompanyName")),
                        PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                        CompanySettingsId = reader.GetInt32(reader.GetOrdinal("CompanySettingsId")),
                        CompanyDefaultsId = reader.GetInt32(reader.GetOrdinal("CompanyDefaultsId")),
                        CompanyAddress1 = reader.GetString(reader.GetOrdinal("CompanyAddress1")),
                        CompanyAddress2 = reader.IsDBNull(reader.GetOrdinal("CompanyAddress2")) ? null : reader.GetString(reader.GetOrdinal("CompanyAddress2")),
                        CompanyCity = reader.GetString(reader.GetOrdinal("CompanyCity")),
                        CompanyZipCode = reader.GetString(reader.GetOrdinal("CompanyZipCode")),
                        CompanyStateCode = reader.GetString(reader.GetOrdinal("CompanyStateCode")),
                        CompanyEmail = reader.GetString(reader.GetOrdinal("CompanyEmail"))
                    };
                }
            }
        }

        private static async Task LoadAllRoomData(ProjectComplexDetails projectComplexDetails, System.Data.Common.DbDataReader reader)
        {
            if (await reader.NextResultAsync())
            {
                while (reader.HasRows && await reader.ReadAsync())
                {
                    var roomDetails = new RoomDetails
                    {
                        RoomId = reader.GetInt32(reader.GetOrdinal("RoomId")),
                        RoomName = reader.GetString(reader.GetOrdinal("RoomName")),
                        Length = reader.GetInt32(reader.GetOrdinal("Length")),
                        Height = reader.GetInt32(reader.GetOrdinal("Height")),
                        Width = reader.GetInt32(reader.GetOrdinal("Width")),
                        IsOptional = reader.GetBoolean(reader.GetOrdinal("IsOptional")),
                        TotalCost = reader.GetFloat(reader.GetOrdinal("TotalCost")),
                        RoomSquareFootage = reader.GetInt32(reader.GetOrdinal("RoomSquareFootage")),
                        IncludeBaseboards = reader.GetBoolean(reader.GetOrdinal("IncludeBaseboards")),
                        IncludeCeilings = reader.GetBoolean(reader.GetOrdinal("IncludeCeilings")),
                        IncludeCrownMoldings = reader.GetBoolean(reader.GetOrdinal("IncludeCrownMoldings")),
                        IncludeDoors = reader.GetBoolean(reader.GetOrdinal("IncludeDoors")),
                        IncludeWalls = reader.GetBoolean(reader.GetOrdinal("IncludeWalls")),
                        IncludeWindows = reader.GetBoolean(reader.GetOrdinal("IncludeWindows"))
                    };
                    projectComplexDetails.RoomDetails.Add(roomDetails);
                }
            }
        }

        private static async Task LoadProjectData(ProjectComplexDetails projectComplexDetails, System.Data.Common.DbDataReader reader)
        {
            // Read Project Details
            if (reader.HasRows && await reader.ReadAsync())
            {
                projectComplexDetails.ProjectDetails = new ProjectDetails
                {
                    ProjectId = reader.GetInt32(reader.GetOrdinal("ProjectId")),
                    CompanyCustomerId = reader.GetInt32(reader.GetOrdinal("CompanyCustomerId")),
                    ProjectName = reader.GetString(reader.GetOrdinal("ProjectName")),
                    Address1 = reader.GetString(reader.GetOrdinal("Address1")),
                    Address2 = reader.IsDBNull(reader.GetOrdinal("Address2")) ? null : reader.GetString(reader.GetOrdinal("Address2")),
                    City = reader.GetString(reader.GetOrdinal("City")),
                    StateCode = reader.GetString(reader.GetOrdinal("StateCode")),
                    ZipCode = reader.GetString(reader.GetOrdinal("ZipCode")),
                    ProjectTotalSquareFootage = reader.GetOrdinal("ProjectTotalSquareFootage")
                };
            }
        }


        // PUT: api/ProjectAreas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProjectArea(int id, ProjectArea projectArea)
        {
            if (id != projectArea.Id)
            {
                return BadRequest();
            }

            _context.Entry(projectArea).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectAreaExists(id))
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

        // POST: api/ProjectAreas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProjectArea>> PostProjectArea(ProjectArea projectArea)
        {
            _context.ProjectAreas.Add(projectArea);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProjectArea", new { id = projectArea.Id }, projectArea);
        }

        // DELETE: api/ProjectAreas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectArea(int id)
        {
            var projectArea = await _context.ProjectAreas.FindAsync(id);
            if (projectArea == null)
            {
                return NotFound();
            }

            _context.ProjectAreas.Remove(projectArea);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectAreaExists(int id)
        {
            return _context.ProjectAreas.Any(e => e.Id == id);
        }
    }
}
