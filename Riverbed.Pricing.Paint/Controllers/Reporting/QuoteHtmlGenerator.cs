using Riverbed.Pricing.Paint.Shared.Entities;
using Riverbed.Pricing.Paint.Shared.Data;
using System.Text;
using System.Linq;

namespace Riverbed.Pricing.Paint.Controllers.Reporting
{
    public static class QuoteHtmlGenerator
    {
        public static string GenerateQuoteHtml(PricingDbContext db, Guid projectGuid)
        {
            var project = db.Projects.FirstOrDefault(p => p.ProjectGuid == projectGuid);
            if (project == null) return "<div>Project not found.</div>";
            var companyCustomer = db.CompanyCustomers.FirstOrDefault(cust => cust.Id == project.CompanyCustomerId);
            var company = companyCustomer != null ? db.Companies.FirstOrDefault(c => c.Id == companyCustomer.CompanyId) : null;
            var rooms = db.Rooms.Where(r => r.ProjectDataId == project.Id).ToList();
            var adjustments = db.Adjustments.Where(a => a.ProjectDataId == project.Id).ToList();
            var discount = adjustments.FirstOrDefault(a => a.Type == AdjustmentType.Discount);
            decimal discountAmount = 0;
            decimal projectTotal = 0;
            decimal projectSubTotal = 0;
            foreach (var room in rooms)
            {
                if (!room.IsOptional)
                    projectSubTotal += (decimal)(room.LaborCost ?? 0) + (decimal)(room.MaterialCost ?? 0) + (decimal)(room.Profit ?? 0) + (decimal)(room.OverheadCost ?? 0);
            }
            projectTotal = projectSubTotal;
            if (discount != null)
            {
                if (discount.IsPercentage)
                    discountAmount = projectTotal * ((decimal)discount.Value / 100);
                else
                    discountAmount = discount.Value;
                projectTotal -= discountAmount;
            }
            var sb = new StringBuilder();
            sb.Append("<div style='width:8.27in; min-height:11.69in; margin:auto; font-family:Segoe UI,Arial,sans-serif;'>");
            // Header
            sb.Append("<div style='display:flex; justify-content:space-between; align-items:flex-start;'>");
            sb.Append("<div>");
            sb.Append($"<div style='font-size:1.5rem; font-weight:bold; margin-bottom:0;'>{company?.CompanyName}</div>");
            sb.Append($"<div>{company?.Address1}</div>");
            if (!string.IsNullOrEmpty(company?.Address2)) sb.Append($"<div>{company.Address2}</div>");
            sb.Append($"<div>{company?.City}, {company?.StateCode} {company?.ZipCode}</div>");
            sb.Append($"<div>Call: {company?.PhoneNumber}</div>");
            sb.Append("</div>");
            sb.Append("<div style='text-align:right;'>");
            sb.Append("<div style='font-size:2.2rem; font-weight:bold; margin:0;'>QUOTE</div>");
            sb.Append($"<div style='font-size:0.95rem; margin-top:0.5rem;'>Created: {DateTime.Now:M/d/yyyy h:mm:ss tt}</div>");
            sb.Append("</div></div>");
            sb.Append("<hr style='margin:1.2rem 0 1.5rem 0; border:0; border-top:2px solid #e0e0e0;' />");
            // Project block
            sb.Append("<div style='margin-bottom:2rem;'><span style='font-weight:bold;'>Project:</span><br/>");
            sb.Append($"<div>{project.Address1}</div>");
            if (!string.IsNullOrEmpty(project.Address2)) sb.Append($"<div>{project.Address2}</div>");
            sb.Append($"<div>{project.City}, {project.StateCode} {project.ZipCode}</div>");
            sb.Append("</div>");
            // Project Areas
            sb.Append("<div style='font-size:1.4rem; font-weight:500; margin-bottom:0.7rem;'>Project Areas</div>");
            sb.Append("<table style='width:100%;border-collapse:collapse;font-size:1rem;'>");
            sb.Append("<thead><tr style='background:#fafafa; border-bottom:2px solid #e0e0e0;'>");
            sb.Append("<th style='text-align:left;padding:8px;font-weight:normal;'>Name</th>");
            sb.Append("<th style='text-align:right;padding:8px;font-weight:normal;'>Optional Cost</th>");
            sb.Append("<th style='text-align:right;padding:8px;font-weight:normal;'>Cost</th>");
            sb.Append("</tr></thead><tbody>");
            int rowIndex = 0;
            foreach (var room in rooms)
            {
                var total = (decimal)(room.LaborCost ?? 0) + (decimal)(room.MaterialCost ?? 0) + (decimal)(room.Profit ?? 0) + (decimal)(room.OverheadCost ?? 0);
                string rowBg = rowIndex % 2 == 1 ? "background:#f5f5f5;" : "";
                sb.Append($"<tr style='border-bottom:1px solid #eee;{rowBg}'>");
                sb.Append("<td style='padding:8px;vertical-align:top;'>");

                // Add room image if available
                if (room.RoomImageId.HasValue)
                {
                    var imageFile = db.DataFiles.FirstOrDefault(f => f.Id == room.RoomImageId.Value);
                    if (imageFile != null)
                    {
                        var base64Image = Convert.ToBase64String(imageFile.FileData);
                        var mimeType = imageFile.FileType ?? "image/jpeg";
                        sb.Append($"<img src='data:{mimeType};base64,{base64Image}' style='max-width:200px;max-height:150px;object-fit:cover;border-radius:4px;margin-bottom:8px;display:block;' alt='{room.Name}' />");
                    }
                }

                sb.Append($"<span style='font-weight:bold;'>{room.Name}</span>");
                if (room.IsOptional) sb.Append(" <span>(Optional)</span>");
                sb.Append("<br/>");
                sb.Append($"<span style='font-size:0.95rem;'>{GetRoomFeatures(room)}</span>");
                if (!string.IsNullOrEmpty(room.Notes)) sb.Append($"<br/><span style='font-size:0.95rem;'>{room.Notes}</span>");
                if (room.PaintableItems != null && room.PaintableItems.Any())
                {
                    foreach (var item in room.PaintableItems)
                    {
                        var totalCost = item.Count > 1 ? item.Count * item.Price : item.Price;
                        sb.Append($"<br/><span style='font-size:0.95rem;'>&#8226; ({item.Count}) {item.Name} - {totalCost.ToString("$#,##0.00")}</span>");
                    }
                }
                sb.Append("</td>");
                sb.Append($"<td style='text-align:right;padding:8px;vertical-align:top;'>{(room.IsOptional ? total.ToString("$#,##0.00") : "$0.00")}</td>");
                sb.Append($"<td style='text-align:right;padding:8px;vertical-align:top;'>{(!room.IsOptional ? total.ToString("$#,##0.00") : "$0.00")}</td>");
                sb.Append("</tr>");
                rowIndex++;
            }
            sb.Append("</tbody></table>");
            // Totals block
            sb.Append("<div style='margin-top:2.5rem; text-align:right;'>");
            sb.Append("<table style='width:100%; border-collapse:collapse; font-size:1.1rem;'>");
            sb.Append("<tr><td style='border:none;'></td><td style='border:none;text-align:right;padding:8px;'>");
            if (discountAmount > 0)
            {
                sb.Append($"Sub Total: {projectSubTotal.ToString("$#,##0.00")}<br/>");
                sb.Append($"Discount: {discountAmount.ToString("$#,##0.00")}<br/>");
            }
            sb.Append($"<span style='font-size:1.3rem;font-weight:bold;'>Total: {projectTotal.ToString("$#,##0.00")}</span>");
            sb.Append("</td></tr></table></div>");
            // End
            sb.Append("</div>");
            return sb.ToString();
        }

        private static string GetRoomFeatures(Room room)
        {
            var features = new List<string>();
            if (room.IncludeWalls) features.Add("Walls");
            if (room.IncludeCeilings) features.Add("Ceilings");
            if (room.IncludeBaseboards) features.Add("Baseboards");
            if (room.IncludeCrownMoldings) features.Add("Crown Molding");
            if (room.IncludeDoors && room.NumberOfDoors > 0) features.Add($"Doors ({room.NumberOfDoors})");
            if (room.IncludeWindows && room.NumberOfWindows > 0) features.Add($"Windows ({room.NumberOfWindows})");
            return string.Join(" / ", features);
        }
    }
}
