using System;
using System.Linq;
using DevExpress.XtraReports.UI;
using Riverbed.Pricing.Paint.Shared.Entities.Reporting;
using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Reports.Data;
using System.Drawing;
using DevExpress.Drawing.Printing;

namespace Riverbed.Pricing.Paint.Reports.PredefinedReports
{
    public class CompanyHTMLReportDevExpress : XtraReport
    {
        public CompanyHTMLReportDevExpress(Guid projectGuid, int reportTypeId, ReportDbContext dbContext)
        {
            // Set up the report's basic layout
            DetailBand detailBand = new DetailBand();
            this.Bands.Add(detailBand);
            this.PaperKind = DXPaperKind.Letter;
            this.Margins.Left = 40;
            this.Margins.Right = 40;
            this.Margins.Top = 40;
            this.Margins.Bottom = 40;

            // Query the CompanyHTMLReport
            var report = dbContext.Set<CompanyHTMLReport>()
                .AsNoTracking()
                .FirstOrDefault(r => r.ProjectGuid == projectGuid && r.ReportTypeId == reportTypeId);

            string htmlText = report?.ReportHTMLText ?? "<div>No report found.</div>";

            // Create XRRichText to render HTML
            XRRichText richText = new XRRichText()
            {
                Html = htmlText,
                BoundsF = new RectangleF(0, 0, 700, 1000),
                Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 10, 10, 10),
                Font = new DevExpress.Drawing.DXFont("Segoe UI", 11F),
                KeepTogether = true,
                CanShrink = true,
                CanGrow = true,
            };

            // Add to DetailBand
            detailBand.Controls.Add(richText);
        }
    }
}
