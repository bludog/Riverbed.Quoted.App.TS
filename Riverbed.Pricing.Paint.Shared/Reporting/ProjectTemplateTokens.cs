using System.Collections.Generic;

namespace Riverbed.Pricing.Paint.Shared.Reporting
{
    /// <summary>
    /// Shared list of available template tokens for project/company HTML reports.
    /// </summary>
    public static class ProjectTemplateTokens
    {
        // Company / Project tokens
        public const string CompanyName = "{{CompanyName}}";
        public const string CustomerGuid = "{{CustomerGuid}}";
        public const string ProjectName = "{{ProjectName}}";
        public const string ProjectGuid = "{{ProjectGuid}}";
        public const string ProjectAddress1 = "{{ProjectAddress1}}";
        public const string ProjectCity = "{{ProjectCity}}";
        public const string ProjectState = "{{ProjectState}}";
        public const string ProjectZip = "{{ProjectZip}}";
        public const string ContactNumber = "{{ContactNumber}}";
        public const string ContactEmail = "{{ContactEmail}}";
        public const string QuoteLink = "{{QuoteLink}}";
        public const string CurrentDate = "{{CurrentDate}}";
        public const string CompanyRepresentativeName = "{{CompanyRepresentativeName}}";
        public const string CompanyPhone = "{{CompanyPhone}}";

        // Customer / Pricing tokens
        public const string ProjectCost = "{{ProjectCost}}";
        public const string ProjectCostHalfPrice = "{{ProjectCostHalfPrice}}";
        public const string FirstName = "{{FirstName}}";
        public const string LastName = "{{LastName}}";
        public const string Email = "{{Email}}";
        public const string CustomerPhone = "{{CustomerPhone}}";

        /// <summary>
        /// Enumerates all tokens in a stable order for display and iteration.
        /// </summary>
        public static IReadOnlyList<string> All { get; } = new[]
        {
            CompanyName, CustomerGuid, CompanyRepresentativeName, CompanyPhone, ProjectName, ProjectGuid, ProjectAddress1, ProjectCity, ProjectState, ProjectZip,
            CustomerPhone, ContactEmail, QuoteLink, CurrentDate,
            ProjectCost, ProjectCostHalfPrice, FirstName, LastName, Email
        };
    }
}