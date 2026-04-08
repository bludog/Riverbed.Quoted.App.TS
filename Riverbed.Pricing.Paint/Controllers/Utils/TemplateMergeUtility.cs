using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Riverbed.Pricing.Paint.Shared.Data;
using Riverbed.Pricing.Paint.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Controllers.Utils
{
    /// <summary>
    /// Utility to merge HTML templates with project/company/customer data by replacing tokens.
    /// </summary>
    public class TemplateMergeUtility
    {
        private readonly PricingDbContext _context;
        private readonly ILogger _logger;

        public TemplateMergeUtility(PricingDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Replaces known tokens in the provided HTML using data from the specified company and project.
        /// </summary>
        public async Task<string> MergeTemplateHtmlAsync(string html, Guid companyGuid, Guid projectGuid)
        {
            if (string.IsNullOrWhiteSpace(html))
                return html;

            try
            {
                // Load project with related data
                var project = await _context.Projects
                    .Include(p => p.Adjustments)
                    .FirstOrDefaultAsync(p => p.ProjectGuid == projectGuid);

                var companyCustomer = project != null
                    ? await _context.CompanyCustomers.FirstOrDefaultAsync(c => c.Id == project.CompanyCustomerId)
                    : null;

                var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == companyGuid);

                // Compute project cost (mirrors quote logic where possible)
                decimal projectSubTotal = 0m;
                decimal projectTotal = 0m;

                if (project != null)
                {
                    if (project.Adjustments != null && project.Adjustments.Any())
                    {
                        projectSubTotal = project.BaseAmount;
                        projectTotal = projectSubTotal;
                        foreach (var adj in project.Adjustments)
                        {
                            try
                            {
                                projectTotal = adj.Apply(projectTotal);
                            }
                            catch
                            {
                                // ignore an invalid adjustment apply
                            }
                        }
                    }
                    else
                    {
                        projectTotal = project.BaseAmount;
                    }
                }

                var halfPrice = projectTotal / 2m;

                var quoteLink = BuildQuoteLink(projectGuid);
                var currentDate = DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

                // Token map
                var replacements = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "{{CustomerGuid}}", project?.ProjectGuid.ToString() ?? string.Empty },
                    { "{{CompanyName}}", company?.CompanyName ?? string.Empty },
                    { "{{ProjectGuid}}", project?.ProjectGuid.ToString() ?? string.Empty },
                    { "{{ProjectName}}", project?.ProjectName ?? string.Empty },
                    { "{{ProjectAddress1}}", project?.Address1 ?? string.Empty },
                    { "{{ProjectCity}}", project?.City ?? string.Empty },
                    { "{{ProjectState}}", project?.StateCode ?? string.Empty },
                    { "{{ProjectZip}}", project?.ZipCode ?? string.Empty },
                    { "{{CustomerPhone}}", companyCustomer?.MobilePhone ?? string.Empty },
                    { "{{ContactEmail}}", company?.Email ?? string.Empty },
                    { "{{QuoteLink}}", quoteLink },
                    { "{{CurrentDate}}", currentDate },
                    { "{{ProjectCost}}", projectTotal == 0 ? string.Empty : projectTotal.ToString("C", CultureInfo.CurrentCulture) },
                    { "{{ProjectCostHalfPrice}}", halfPrice == 0 ? string.Empty : halfPrice.ToString("C", CultureInfo.CurrentCulture) },
                    { "{{FirstName}}", companyCustomer?.FirstName ?? string.Empty },
                    { "{{LastName}}", companyCustomer?.LastName ?? string.Empty },
                    { "{{Email}}", companyCustomer?.Email ?? string.Empty },
                    { "{{CompanyRepresentativeName}}", company?.CompanyRepresentativeName ?? string.Empty },
                    { "{{CompanyPhone}}", company?.PhoneNumber ?? string.Empty }, // legacy support
                };

                foreach (var kvp in replacements)
                {
                    if (!string.IsNullOrEmpty(kvp.Key))
                        html = html.Replace(kvp.Key, kvp.Value, StringComparison.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error merging template HTML for project {ProjectGuid}", projectGuid);
            }

            return html;
        }

        private static string BuildQuoteLink(Guid projectGuid)
        {
#if DEBUG
            const string baseUrl = "https://localhost:7027";
#else
            const string baseUrl = "https://bludog-software.com/rbp";
#endif
            return $"{baseUrl}/reports/project-quote-report-main/{projectGuid}";
        }
    }
}