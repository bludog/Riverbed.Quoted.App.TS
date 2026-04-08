using System;
using System.Collections.Generic;

namespace Riverbed.Pricing.Paint.Shared.Data
{
    /// <summary>
    /// Dashboard statistics and project lists for the company dashboard
    /// </summary>
    public record DashboardData
    {
        public string CompanyName { get; init; } = string.Empty;

        // Year statistics
        public int WonQuotesYear { get; init; }
        public decimal WonQuotesTotalYear { get; init; }
        public int ActiveQuotesYear { get; init; }
        public decimal ActiveQuotesTotalYear { get; init; }
        public int CompletedJobsYear { get; init; }
        public decimal CompletedJobsTotalYear { get; init; }

        // Month statistics
        public int WonQuotesMonth { get; init; }
        public decimal WonQuotesTotalMonth { get; init; }
        public int ActiveQuotesMonth { get; init; }
        public decimal ActiveQuotesTotalMonth { get; init; }
        public int CompletedJobsMonth { get; init; }
        public decimal CompletedJobsTotalMonth { get; init; }

        // Project lists
        public List<ProjectDataMinimal> WonProjects { get; init; } = [];
        public List<ProjectDataMinimal> PendingProjects { get; init; } = [];
    }
}
