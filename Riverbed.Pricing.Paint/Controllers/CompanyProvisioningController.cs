using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Entities;
using Riverbed.Pricing.Paint.Shared;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;
using Riverbed.Pricing.Paint.Shared.Entities.Reporting;

namespace Riverbed.Pricing.Paint.Controllers;

/// <summary>
/// Centralized controller for provisioning all default data when a new company is created.
/// Single entry point replaces the scattered default-seeding calls across multiple controllers.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CompanyProvisioningController : ControllerBase
{
    private readonly PricingDbContext _db;
    private readonly ILogger<CompanyProvisioningController> _logger;

    /// <summary>
    /// Well-known GUID used as the "template company" for copying paintable items.
    /// </summary>
    private static readonly Guid TemplateCompanyId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public CompanyProvisioningController(PricingDbContext db, ILogger<CompanyProvisioningController> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Provisions all default data for a newly created company.
    /// Idempotent — safe to call again if a previous run partially failed.
    /// </summary>
    [HttpPost("{companyGuid}")]
    public async Task<IActionResult> ProvisionCompanyAsync(string companyGuid)
    {
        if (!Guid.TryParse(companyGuid, out var companyId))
            return BadRequest("Invalid company GUID.");

        var company = await _db.Companies.FindAsync(companyId);
        if (company == null)
            return NotFound($"Company {companyGuid} not found.");

        _logger.LogInformation("Starting provisioning for company {CompanyId} ({CompanyName})", companyId, company.CompanyName);

        var results = new Dictionary<string, string>();

        results["CompanySettings"] = await RunStepAsync(() => SeedCompanySettingsAsync(companyId));
        results["InteriorPricing"] = await RunStepAsync(() => SeedInteriorPricingAsync(companyId));
        results["ExteriorPricing"] = await RunStepAsync(() => SeedExteriorPricingAsync(companyId));
        results["CompanyDefaults"] = await RunStepAsync(() => SeedCompanyDefaultsAsync(companyId));
        results["RoomDefaults"] = await RunStepAsync(() => SeedRoomDefaultsAsync(companyId));
        results["CompanyPaintTypes"] = await RunStepAsync(() => SeedCompanyPaintTypesAsync(companyId));
        results["CompanyPaintableItems"] = await RunStepAsync(() => SeedPaintableItemsAsync(companyId));
        results["ServiceTitanConnection"] = await RunStepAsync(() => SeedServiceTitanAsync(companyId, company.CompanyName));
        results["HTMLReportTemplates"] = await RunStepAsync(() => SeedHtmlReportTemplatesAsync(companyId, company.CompanyName));

        var failed = results.Where(r => r.Value != "OK").ToList();
        if (failed.Count > 0)
        {
            _logger.LogWarning("Provisioning completed with {FailedCount} failures for company {CompanyId}", failed.Count, companyId);
            return StatusCode(StatusCodes.Status207MultiStatus, results);
        }

        _logger.LogInformation("Provisioning completed successfully for company {CompanyId}", companyId);
        return Ok(results);
    }

    /// <summary>
    /// Runs a provisioning step, returning "OK" or the error message.
    /// Ensures one failing step does not block the rest.
    /// </summary>
    private async Task<string> RunStepAsync(Func<Task> step)
    {
        try
        {
            await step();
            return "OK";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Provisioning step failed: {Message}", ex.Message);
            return ex.Message;
        }
    }

    // ───────────────────────────────────────────────────────────
    //  Individual seed methods — each is idempotent
    // ───────────────────────────────────────────────────────────

    private async Task SeedCompanySettingsAsync(Guid companyId)
    {
        if (await _db.CompanySettings.AnyAsync(c => c.CompanyId == companyId))
            return;

        _db.CompanySettings.Add(new CompanySettings
        {
            CompanyId = companyId,
            LaborPercentage = 42,
            HourlyRate = 75,
            MaterialPercentage = 13,
            OverheadPercentage = 45
        });
        await _db.SaveChangesAsync();
    }

    private async Task SeedInteriorPricingAsync(Guid companyId)
    {
        if (await _db.PricingInteriorDefaults.AnyAsync(c => c.CompanyId == companyId))
            return;

        _db.PricingInteriorDefaults.Add(new PricingInteriorDefault
        {
            CompanyId = companyId,
            WallRatePerSquareFoot = 100f,
            CeilingRatePerSquareFoot = 80f,
            BaseboardRatePerLinearFoot = 50f,
            DoorRateEach = 0.5f,
            CrownMoldingRatePerLinearFoot = 35f,
            WindowRateEach = 35f,
            PaintCoveragePerGallon = 350,
            PaintCoats = 2
        });
        await _db.SaveChangesAsync();
    }

    private async Task SeedExteriorPricingAsync(Guid companyId)
    {
        if (await _db.PricingExteriorDefaults.AnyAsync(c => c.CompanyId == companyId))
            return;

        _db.PricingExteriorDefaults.Add(new PricingExteriorDefault
        {
            CompanyId = companyId,
            ExteriorDoorRate = 60.0f,
            SingleGarageDoorRate = 100.0f,
            DoubleGarageDoorRate = 150.0f,
            BoxingRatePerLinearFoot = 4.0f,
            SidingRatePerSquareFoot = 1.75f,
            ChimneyRateFlat = 300.0f,
            PaintCoveragePerGallon = 350,
            PaintCoats = 2
        });
        await _db.SaveChangesAsync();
    }

    private async Task SeedCompanyDefaultsAsync(Guid companyId)
    {
        if (await _db.CompanyDefaults.AnyAsync(c => c.CompanyId == companyId))
            return;

        // Copy from the first existing CompanyDefaults row as a template
        var template = await _db.CompanyDefaults.FirstOrDefaultAsync();

        _db.CompanyDefaults.Add(new CompanyDefaults
        {
            CompanyId = companyId,
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
        });
        await _db.SaveChangesAsync();
    }

    private async Task SeedRoomDefaultsAsync(Guid companyId)
    {
        if (await _db.RoomGlobalDefaults.AnyAsync(c => c.CompanyId == companyId))
            return;

        _db.RoomGlobalDefaults.Add(new RoomGlobalDefaults
        {
            CompanyId = companyId,
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
            PaintQualityId = 2
        });
        await _db.SaveChangesAsync();
    }

    private async Task SeedCompanyPaintTypesAsync(Guid companyId)
    {
        if (await _db.CompanyPaintTypes.AnyAsync(c => c.CompanyId == companyId))
            return;

        var globalPaints = await _db.PaintTypes.ToListAsync();
        foreach (var paint in globalPaints)
        {
            _db.CompanyPaintTypes.Add(new CompanyPaintType
            {
                CompanyId = companyId,
                PaintTypeName = paint.PaintTypeName,
                CoverageOneCoatSqFt = paint.CoverageOneCoatSqFt,
                CoverageTwoCoatsSqFt = paint.CoverageTwoCoatsSqFt,
                PricePerGallon = paint.PricePerGallon,
                PaintSheenId = paint.PaintSheenId,
                PaintBrandId = paint.PaintBrandId
            });
        }
        await _db.SaveChangesAsync();
    }

    private async Task SeedPaintableItemsAsync(Guid companyId)
    {
        if (await _db.CompanyPaintableItems.AnyAsync(c => c.CompanyId == companyId))
            return;

        var templateItems = await _db.CompanyPaintableItems
            .Where(pi => pi.CompanyId == TemplateCompanyId)
            .ToListAsync();

        foreach (var item in templateItems)
        {
            _db.CompanyPaintableItems.Add(new CompanyPaintableItem
            {
                CompanyId = companyId,
                Name = item.Name,
                Description = item.Description,
                PaintTypeId = item.PaintTypeId,
                PricingTypeId = item.PricingTypeId,
                Price = item.Price,
                SquareFootage = item.SquareFootage,
                BaseTime = item.BaseTime,
                PaintableItemCategoryId = item.PaintableItemCategoryId
            });
        }
        await _db.SaveChangesAsync();
    }

    private async Task SeedServiceTitanAsync(Guid companyId, string companyName)
    {
        if (await _db.ServiceTitanConnectionDatas.AnyAsync(c => c.CompanyGuid == companyId))
            return;

        _db.ServiceTitanConnectionDatas.Add(new ServiceTitanConnectionData
        {
            CompanyGuid = companyId,
            CompanyName = companyName,
            ServiceTitanApiUrl = "https://auth-integration.servicetitan.io",
            ServiceTitanApiVersion = "v2",
            ServiceTitanSecretKey = "cs1.000000000000",
            ServiceTitanApiClientId = "cid.000000000000",
            ServiceTitanTenantId = 1000000000,
            ServiceTitanAppKey = "ak1.000000000000"
        });
        await _db.SaveChangesAsync();
    }

    private async Task SeedHtmlReportTemplatesAsync(Guid companyId, string companyName)
    {
        var globalTemplates = await _db.CompanyHTMLReportsTemplate
            .Where(t => t.IsAppGlobalTemplate)
            .ToListAsync();

        if (globalTemplates.Count == 0)
        {
            _logger.LogWarning("No global HTML report templates found (IsAppGlobalTemplate=true). Skipping template copy for company {CompanyId}", companyId);
            return;
        }

        foreach (var template in globalTemplates)
        {
            var exists = await _db.CompanyHTMLReportsTemplate.AnyAsync(t =>
                t.CompanyGuid == companyId &&
                t.ReportName == template.ReportName &&
                t.ReportTypeId == template.ReportTypeId);

            if (exists)
                continue;

            // Replace any hardcoded source company name in the HTML with the new company name
            var html = template.ReportHTMLText ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(template.CompanyName)
                && !string.IsNullOrWhiteSpace(companyName)
                && !string.Equals(template.CompanyName, companyName, StringComparison.OrdinalIgnoreCase))
            {
                html = html.Replace(template.CompanyName, companyName);
            }

            _db.CompanyHTMLReportsTemplate.Add(new CompanyHTMLReportTemplate
            {
                CompanyGuid = companyId,
                CompanyName = companyName,
                ReportName = template.ReportName,
                ReportTypeId = template.ReportTypeId,
                ReportHTMLText = html,
                IsGlobalTemplate = false,
                IsAppGlobalTemplate = false,
                IsActive = template.IsActive,
                DisplayOrder = template.DisplayOrder,
                LastUpdatedDateTime = DateTime.UtcNow
            });
        }
        await _db.SaveChangesAsync();
        _logger.LogInformation("Copied {Count} HTML report templates for company {CompanyId} ({CompanyName})", globalTemplates.Count, companyId, companyName);
    }
}
