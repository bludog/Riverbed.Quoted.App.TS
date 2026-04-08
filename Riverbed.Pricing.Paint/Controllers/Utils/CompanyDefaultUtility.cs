using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Entities;
using Riverbed.Pricing.Paint.Shared;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;
using Riverbed.Pricing.Paint.Shared.Entities.Reporting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Controllers.Utils
{
    public class CompanyDefaultUtility
    {
        private Guid _companyGuid;
        private readonly PricingDbContext _context;
        private readonly ILogger<CompaniesController> _logger;

        public CompanyDefaultUtility(PricingDbContext context, ILogger<CompaniesController> logger, Guid companyGuid)
        {
            _companyGuid = companyGuid;
            _context = context;
            _logger = logger;
        }

        // Set Default Values for CompanySettings
        public async Task SetCompanySettings()
        {
            // Check if defaults are already set
            var defaultsSet = _context.CompanySettings.Any(c => c.CompanyId == _companyGuid);
            if (defaultsSet)
            {
                _logger.LogInformation($"Defaults already set for company with ID: {_companyGuid}");
                return;
            }

            _logger.LogInformation($"Setting default values for CompanySettings for company: {_companyGuid}");
            var companySettings = new CompanySettings
            {
                CompanyId = _companyGuid,
                LaborPercentage = 42, 
                HourlyRate = 75, 
                MaterialPercentage = 13, 
                OverheadPercentage = 45, 
            };
            _context.CompanySettings.Add(companySettings);
            _context.SaveChanges();
            _logger.LogInformation($"Default values for CompanySettings set for company: {_companyGuid}");
        }

        // Delete Default Values for CompanySettings
        public async Task DeleteCompanySettings()
        {
            _logger.LogInformation($"Deleting default values for CompanySettings for company: {_companyGuid}");
            // Check if defaults are already set
            var defaultsSet = _context.CompanySettings.Any(c => c.CompanyId == _companyGuid);
            if (!defaultsSet)
            {
                _logger.LogInformation($"No defaults found for company with ID: {_companyGuid}");
                return;
            }
            _context.CompanySettings.RemoveRange(_context.CompanySettings.Where(c => c.CompanyId == _companyGuid));
            await _context.SaveChangesAsync();
        }

        // Set Default Values for CompanyInteriorPricing
        public async Task SetCompanyInteriorPricing()
        {
            // Check if defaults are already set
            var defaultsSet = _context.PricingInteriorDefaults.Any(c => c.CompanyId == _companyGuid);
            if (defaultsSet)
            {
                _logger.LogInformation($"Defaults already set for company with ID: {_companyGuid}");
                return;
            }

            _logger.LogInformation($"Setting default values for CompanyInteriorPricing for company: {_companyGuid}");
            var companyInteriorPricing = new PricingInteriorDefault
            {
                CompanyId = _companyGuid,
                BaseboardRatePerLinearFoot = 50f,
                CeilingRatePerSquareFoot = 80f,
                DoorRateEach = 0.5f,
                WallRatePerSquareFoot = 100f,
                WindowRateEach = 35f,
                CrownMoldingRatePerLinearFoot = 35f,
                PaintCoveragePerGallon = 350,
                PaintCoats = 2
            };
           _context.PricingInteriorDefaults.Add(companyInteriorPricing);
           _context.SaveChanges();
            _logger.LogInformation($"Default values for CompanyInteriorPricing set for company: {_companyGuid}");
        }

        // Delete Default Values for CompanyInteriorPricing
        public async Task DeleteCompanyInteriorPricing()
        {
            _logger.LogInformation($"Deleting default values for CompanyInteriorPricing for company: {_companyGuid}");
            // Check if defaults are already set
            var defaultsSet = _context.PricingInteriorDefaults.Any(c => c.CompanyId == _companyGuid);
            if (!defaultsSet)
            {
                _logger.LogInformation($"No defaults found for company with ID: {_companyGuid}");
                return;
            }
            _context.PricingInteriorDefaults.RemoveRange(_context.PricingInteriorDefaults.Where(c => c.CompanyId == _companyGuid));
            await _context.SaveChangesAsync();
        }

        // Set Default Values for CompanyExteriorPricing
        public async Task SetCompanyExteriorPricing()
        {
            // Check if defaults are already set
            var defaultsSet = _context.PricingExteriorDefaults.Any(c => c.CompanyId == _companyGuid);
            if (defaultsSet)
            {
                _logger.LogInformation($"Defaults already set for company with ID: {_companyGuid}");
                return;
            }

            _logger.LogInformation($"Setting default values for CompanyExteriorPricing for company: {_companyGuid}");
            var companyExteriorPricing = new PricingExteriorDefault
            {
                CompanyId = _companyGuid,
                ExteriorDoorRate = 60.0f,
                SingleGarageDoorRate = 100.0f,
                DoubleGarageDoorRate = 150.0f,
                BoxingRatePerLinearFoot = 4.0f,
                SidingRatePerSquareFoot = 1.75f,
                ChimneyRateFlat = 300.0f,
                PaintCoveragePerGallon = 350,
                PaintCoats = 2
            };
            _context.PricingExteriorDefaults.Add(companyExteriorPricing);
            _context.SaveChanges();
            _logger.LogInformation($"Default values for CompanyExteriorPricing set for company: {_companyGuid}");
        }

        //Delete Default Values for CompanyExteriorPricing
        public async Task DeleteCompanyExteriorPricing()
        { 
            _logger.LogInformation($"Deleting default values for CompanyExteriorPricing for company: {_companyGuid}");
            // Check if defaults are already set
            var defaultsSet = _context.PricingExteriorDefaults.Any(c => c.CompanyId == _companyGuid);
            if (!defaultsSet)
            {
                _logger.LogInformation($"No defaults found for company with ID: {_companyGuid}");
                return;
            }

            _context.RemoveRange(_context.PricingExteriorDefaults.Where(c => c.CompanyId == _companyGuid));
            await _context.SaveChangesAsync();
        }

        // Set Default Values for CompanyPaintableItems
        public async Task SetCompanyPaintableItems()
        {

            if (_context.Database.GetDbConnection().State != ConnectionState.Open)
            {
                _context.Database.OpenConnection();
                _logger.LogInformation("The database connection has been opened.");
            }


            // Check if defaults are already set
            var hasItems = _context.CompanyPaintableItems.Any(c => c.CompanyId == _companyGuid);
            if (hasItems)
            {
                _logger.LogInformation($"Defaults already set for company with ID: {_companyGuid}");
                return;
            }

            _logger.LogInformation($"Setting default values for CompanyPaintableItems for company: {_companyGuid}");
            // Add default paintable items for the company
            var defaultCompanyId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // Assuming this is the default company ID
     
            var defaultPaintableItems = _context.CompanyPaintableItems.Where(pi => pi.CompanyId == defaultCompanyId);

            foreach (var item in defaultPaintableItems)
            {
                var newItem = new CompanyPaintableItem
                {
                    CompanyId = _companyGuid,
                    Name = item.Name,
                    Description = item.Description,
                    PaintTypeId = item.PaintTypeId,
                    PricingTypeId = item.PricingTypeId,
                    Price = item.Price,
                    SquareFootage = item.SquareFootage,
                    BaseTime = item.BaseTime,
                    PaintableItemCategoryId = item.PaintableItemCategoryId,
                };
                _context.CompanyPaintableItems.Add(newItem);
            }
            _context.SaveChanges();
            _logger.LogInformation($"Default paintable items set for company: {_companyGuid}");
        }

        // Delete Default Values for CompanyPaintableItems
        public async Task DeleteCompanyPaintableItems()
        {
            _logger.LogInformation($"Deleting default values for CompanyPaintableItems for company: {_companyGuid}");
            // Check if defaults are already set
            var defaultsSet = _context.CompanyPaintableItems.Any(c => c.CompanyId == _companyGuid);
            if (!defaultsSet)
            {
                _logger.LogInformation($"No defaults found for company with ID: {_companyGuid}");
                return;
            }
            _context.RemoveRange(_context.CompanyPaintableItems.Where(c => c.CompanyId == _companyGuid));
            await _context.SaveChangesAsync();
        }

        // Set Default Values for CompanyDefaults
        public async Task SetCompanyRoomDefaults()
        {
            // Check if defaults are already set
            var defaultsSet =  _context.RoomGlobalDefaults.Any(c => c.CompanyId == _companyGuid);
            if (defaultsSet)
            {
                _logger.LogInformation($"Defaults already set for company with ID: {_companyGuid}");
                return;
            }

            // Set default values for RoomGlobalDefaults
            var roomGlobalDefaults = new RoomGlobalDefaults
            {
                Length = 11,
                Width = 12,
                Height = 9,
                NumberOfDoors = 2,
                IncludeCeilings = true,
                IncludeBaseboards = true,
                IncludeCrownMoldings = true,
                IncludeWalls = true,
                IncludeDoors = true,
                IncludeWindows = true,
                PaintQualityId = 2, // Assuming a default paint quality ID
                CompanyId = _companyGuid
            };
            _context.RoomGlobalDefaults.Add(roomGlobalDefaults);
            _context.SaveChanges();
            _logger.LogInformation($"Default room global defaults set for company: {_companyGuid}");
        }

        /// <summary>
        /// Seeds CompanyDefaults row for a new company, copying values from the first existing row
        /// or using sensible defaults if no rows exist.
        /// </summary>
        public async Task SetCompanyDefaultsRecord()
        {
            var exists = await _context.CompanyDefaults.AnyAsync(c => c.CompanyId == _companyGuid);
            if (exists)
            {
                _logger.LogInformation("CompanyDefaults already exist for company {CompanyGuid}", _companyGuid);
                return;
            }

            // Copy from the first existing CompanyDefaults row as a template
            var template = await _context.CompanyDefaults.FirstOrDefaultAsync();

            var companyDefaults = new CompanyDefaults
            {
                CompanyId = _companyGuid,
                Baseboards = template?.Baseboards ?? true,
                Ceilings = template?.Ceilings ?? true,
                Doors = template?.Doors ?? true,
                Walls = template?.Walls ?? true,
                Windows = template?.Windows ?? true,
                TrimDoors = template?.TrimDoors ?? true,
                PaintTypeCeilingsId = template?.PaintTypeCeilingsId ?? 0,
                PaintTypeWallsId = template?.PaintTypeWallsId ?? 0,
                PaintTypeBaseboardsId = template?.PaintTypeBaseboardsId ?? 0,
                PaintTypeTrimDoorsId = template?.PaintTypeTrimDoorsId ?? 0,
                PaintTypeWindowsId = template?.PaintTypeWindowsId ?? 0
            };

            _context.CompanyDefaults.Add(companyDefaults);
            await _context.SaveChangesAsync();
            _logger.LogInformation("CompanyDefaults seeded for company {CompanyGuid}", _companyGuid);
        }

        // Delete Default Values for CompanyRoomDefaults
        public async Task DeleteCompanyRoomDefaults()
        {
            _logger.LogInformation($"Deleting default values for CompanyRoomDefaults for company: {_companyGuid}");
            // Check if defaults are already set
            var defaultsSet = _context.RoomGlobalDefaults.Any(c => c.CompanyId == _companyGuid);
            if (!defaultsSet)
            {
                _logger.LogInformation($"No defaults found for company with ID: {_companyGuid}");
                return;
            }
            _context.RemoveRange(_context.RoomGlobalDefaults.Where(c => c.CompanyId == _companyGuid));
            await _context.SaveChangesAsync();
        }

        // Set Default Values for CompanyPaintType
        public async Task SetCompanyPaintType()
        {
            // Check if defaults are already set
            var defaultsSet = _context.
                CompanyPaintTypes.Any(c => c.CompanyId == _companyGuid);
            if (defaultsSet)
            {
                _logger.LogInformation($"Defaults already set for company with ID: {_companyGuid}");
                return;
            }

            _logger.LogInformation($"Setting default values for CompanyPaintType for company: {_companyGuid}");
            var globalPaints = _context.PaintTypes.ToList();

            foreach (var paint in globalPaints)
            {
                var companyPaintType = new CompanyPaintType
                {
                    CompanyId = _companyGuid,
                    PaintTypeName = paint.PaintTypeName,
                    CoverageOneCoatSqFt = paint.CoverageOneCoatSqFt,
                    CoverageTwoCoatsSqFt = paint.CoverageTwoCoatsSqFt,
                    PricePerGallon = paint.PricePerGallon,
                    PaintSheenId = paint.PaintSheenId,
                    PaintBrandId = paint.PaintBrandId
                };
                _context.CompanyPaintTypes.Add(companyPaintType);
            }
            _context.SaveChanges();
            _logger.LogInformation($"Default paint types set for company: {_companyGuid}");
        }

        // Delete Default Values for CompanyPaintType
        public async Task DeleteCompanyPaintType()
        {
            _logger.LogInformation($"Deleting default values for CompanyPaintType for company: {_companyGuid}");
            // Check if defaults are already set
            var defaultsSet = _context.CompanyPaintTypes.Any(c => c.CompanyId == _companyGuid);
            if (!defaultsSet)
            {
                _logger.LogInformation($"No defaults found for company with ID: {_companyGuid}");
                return;
            }
            _context.RemoveRange(_context.CompanyPaintTypes.Where(c => c.CompanyId == _companyGuid));
            await _context.SaveChangesAsync();
        }

        // Set ServiceTitan Defaults
        public async Task SetServiceTitanDefaults(string companyName)
        {
            _logger.LogInformation($"Setting default values for ServiceTitan for company: {_companyGuid}");
            var serviceTitanConnectionData = new ServiceTitanConnectionData
            {
                ServiceTitanApiUrl = "https://auth-integration.servicetitan.io",
                ServiceTitanApiVersion = "v2",
                ServiceTitanSecretKey = "cs1.000000000000",
                CompanyName = companyName,
                CompanyGuid = _companyGuid,
                ServiceTitanApiClientId = "cid.000000000000",
                ServiceTitanTenantId = 1000000000,
                ServiceTitanAppKey = "ak1.000000000000"
            };
           
            _context.ServiceTitanConnectionDatas.Add(serviceTitanConnectionData);
            _context.SaveChanges();
            _logger.LogInformation($"Default ServiceTitan values set for company: {_companyGuid}");
        }

        // Delete ServiceTitan Defaults 
        public async Task DeleteServiceTitanDefaults()
        {
            _logger.LogInformation($"Deleting default values for ServiceTitan for company: {_companyGuid}");
            // Check if defaults are already set
            var defaultsSet = _context.ServiceTitanConnectionDatas.Any(c => c.CompanyGuid == _companyGuid);
            if (!defaultsSet)
            {
                _logger.LogInformation($"No defaults found for company with ID: {_companyGuid}");
                return;
            }
            _context.RemoveRange(_context.ServiceTitanConnectionDatas.Where(c => c.CompanyGuid == _companyGuid));
            await _context.SaveChangesAsync();
        }

        // Copy all global CompanyHTMLReportTemplate entries to a specific company
        public async Task CopyGlobalCompanyHTMLReportTemplatesToCompany()
        {
            // Get the company
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == _companyGuid);
            if (company == null)
            {
                _logger.LogWarning($"Company with ID {_companyGuid} not found. Cannot copy global report templates.");
                return;
            }

            // Get all global templates
            var globalTemplates = await _context.CompanyHTMLReportsTemplate
                .Where(t => t.IsAppGlobalTemplate)
                .ToListAsync();

            foreach (var template in globalTemplates)
            {
                // Check if a template with the same ReportName and ReportTypeId already exists for this company
                var exists = await _context.CompanyHTMLReportsTemplate.AnyAsync(t =>
                    t.CompanyGuid == _companyGuid &&
                    t.ReportName == template.ReportName &&
                    t.ReportTypeId == template.ReportTypeId);
                if (exists)
                    continue;

                // Replace any hardcoded source company name in the HTML with the new company name
                var html = template.ReportHTMLText ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(template.CompanyName)
                    && !string.IsNullOrWhiteSpace(company.CompanyName)
                    && !string.Equals(template.CompanyName, company.CompanyName, StringComparison.OrdinalIgnoreCase))
                {
                    html = html.Replace(template.CompanyName, company.CompanyName);
                }

                var newTemplate = new CompanyHTMLReportTemplate
                {
                    CompanyGuid = _companyGuid,
                    CompanyName = company.CompanyName,
                    ReportName = template.ReportName,
                    ReportTypeId = template.ReportTypeId,
                    ReportHTMLText = html,
                    IsGlobalTemplate = false, // Mark as company-specific
                    IsAppGlobalTemplate = false,
                    IsActive = template.IsActive,
                    DisplayOrder = template.DisplayOrder,
                    LastUpdatedDateTime = DateTime.UtcNow
                };
                _context.CompanyHTMLReportsTemplate.Add(newTemplate);
            }
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Copied {globalTemplates.Count} global CompanyHTMLReportTemplates to company: {_companyGuid}");
        }

    }
}
