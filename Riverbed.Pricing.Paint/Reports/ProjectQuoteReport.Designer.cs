using DevExpress.XtraReports.UI;
using System.ComponentModel;

namespace Riverbed.Pricing.Paint.Reports
{
    partial class ProjectQuoteReport : DevExpress.XtraReports.UI.XtraReport, IDisposable
    {
        private System.ComponentModel.IContainer components = null;

        // Declare all UI components
        public TopMarginBand TopMargin { get; private set; }
        public BottomMarginBand BottomMargin { get; private set; }
        public ReportHeaderBand ReportHeader { get; private set; }
        public XRPanel xrPanel1 { get; private set; }
        public XRLabel xrLabel1 { get; private set; }
        public XRPictureBox xrPictureBox1 { get; private set; }
        public PageHeaderBand PageHeader { get; private set; }
        public XRPanel xrPanel3 { get; private set; }
        public XRLabel xrLabel6 { get; private set; }
        public XRLabel xrLabel7 { get; private set; }
        public XRLabel xrLabel10 { get; private set; }
        public XRLabel xrLabel12 { get; private set; }
        public DetailReportBand DetailReport { get; private set; }
        public DetailBand Detail1 { get; private set; }
        public XRTable xrTable2 { get; private set; }
        public XRTableRow xrTableRow2 { get; private set; }
        public XRTableCell xrTableCell4 { get; private set; }
        public XRTableCell xrTableCell5 { get; private set; }
        public XRTableCell xrTableCell6 { get; private set; }
        public GroupHeaderBand GroupHeader2 { get; private set; }
        public XRLabel xrLabel2 { get; private set; }
        public XRLabel xrLabel3 { get; private set; }
        public XRLabel xrLabel17 { get; private set; }
        public GroupFooterBand GroupFooter1 { get; private set; }
        public XRLabel xrLabel24 { get; private set; }
        public XRLabel xrLabel25 { get; private set; }
        public XRLine xrLine1 { get; private set; }
        public XRLabel xrLabel19 { get; private set; }
        public XRLabel xrLabel18 { get; private set; }
        public PageFooterBand PageFooter { get; private set; }
        public XRLabel xrLabel14 { get; private set; }

        /// <summary>
        /// Disposes the resources used by the report.
        /// </summary>
        public new void Dispose()
        {
            Dispose(true);
            // Suppress finalization to prevent the garbage collector from calling the destructor
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the resources used by the report.
        /// </summary>
        /// <param name="disposing">True if called explicitly, false if called by the garbage collector.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose the components if they are not null
                if (components != null)
                {
                    components.Dispose();
                    components = null; // Set to null to avoid potential reuse
                }
            }
            // Call the base class Dispose method
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // Initialize all UI components
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.xrPanel1 = new DevExpress.XtraReports.UI.XRPanel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPictureBox1 = new DevExpress.XtraReports.UI.XRPictureBox();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.xrPanel3 = new DevExpress.XtraReports.UI.XRPanel();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel10 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel12 = new DevExpress.XtraReports.UI.XRLabel();
            this.DetailReport = new DevExpress.XtraReports.UI.DetailReportBand();
            this.Detail1 = new DevExpress.XtraReports.UI.DetailBand();
            this.xrTable2 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell4 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell5 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell6 = new DevExpress.XtraReports.UI.XRTableCell();
            this.GroupHeader2 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel17 = new DevExpress.XtraReports.UI.XRLabel();
            this.GroupFooter1 = new DevExpress.XtraReports.UI.GroupFooterBand();
            this.xrLabel24 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel25 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel19 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel18 = new DevExpress.XtraReports.UI.XRLabel();
            this.PageFooter = new DevExpress.XtraReports.UI.PageFooterBand();
            this.xrLabel14 = new DevExpress.XtraReports.UI.XRLabel();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();

            // TopMargin
            this.TopMargin.HeightF = 10F;
            this.TopMargin.Name = "TopMargin";

            // BottomMargin
            this.BottomMargin.HeightF = 10F;
            this.BottomMargin.Name = "BottomMargin";

            // ReportHeader
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.xrPanel1});
            this.ReportHeader.Name = "ReportHeader";

            // xrPanel1
            this.xrPanel1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.xrLabel1,
                this.xrPictureBox1});
            this.xrPanel1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrPanel1.Name = "xrPanel1";
            this.xrPanel1.SizeF = new System.Drawing.SizeF(830F, 100F);

            // xrLabel1
            this.xrLabel1.Font = new DevExpress.Drawing.DXFont("Lucida Sans Typewriter", 26F, DevExpress.Drawing.DXFontStyle.Bold);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(671.875F, 10F);
            this.xrLabel1.Multiline = true;
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(148.125F, 47.70832F);
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.Text = "QUOTE";

            // xrPictureBox1
            this.xrPictureBox1.ImageUrl = "https://www.example.com/logo.png"; // Update with your logo URL
            this.xrPictureBox1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrPictureBox1.Name = "xrPictureBox1";
            this.xrPictureBox1.SizeF = new System.Drawing.SizeF(205.2083F, 100F);
            this.xrPictureBox1.Sizing = DevExpress.XtraPrinting.ImageSizeMode.ZoomImage;

            // PageHeader
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.xrPanel3});
            this.PageHeader.HeightF = 131.6667F;
            this.PageHeader.Name = "PageHeader";

            // xrPanel3
            this.xrPanel3.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.xrLabel6,
                this.xrLabel7,
                this.xrLabel10,
                this.xrLabel12});
            this.xrPanel3.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrPanel3.Name = "xrPanel3";
            this.xrPanel3.SizeF = new System.Drawing.SizeF(830F, 131.6667F);

            // xrLabel6
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(4.166667F, 28.74998F);
            this.xrLabel6.Multiline = true;
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(100F, 19.875F);
            this.xrLabel6.Text = "Project:";

            // xrLabel7
            this.xrLabel7.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ProjectName]")}); // Remap to ProjectQuoteReport FieldList
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(119.7917F, 28.74998F);
            this.xrLabel7.Multiline = true;
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel7.SizeF = new System.Drawing.SizeF(195.4168F, 19.875F);

            // xrLabel10
            this.xrLabel10.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Address1]")}); // Remap to ProjectQuoteReport FieldList
            this.xrLabel10.LocationFloat = new DevExpress.Utils.PointFloat(119.7918F, 48.62496F);
            this.xrLabel10.Multiline = true;
            this.xrLabel10.Name = "xrLabel10";
            this.xrLabel10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel10.SizeF = new System.Drawing.SizeF(195.4168F, 16.74997F);

            // xrLabel12
            this.xrLabel12.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[City] + ', ' + [StateCode] + ' ' + [ZipCode]")}); // Remap to ProjectQuoteReport FieldList
            this.xrLabel12.LocationFloat = new DevExpress.Utils.PointFloat(118.9584F, 65.37498F);
            this.xrLabel12.Multiline = true;
            this.xrLabel12.Name = "xrLabel12";
            this.xrLabel12.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel12.SizeF = new System.Drawing.SizeF(314.1668F, 16.74997F);

            // DetailReport
            this.DetailReport.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
                this.Detail1,
                this.GroupHeader2,
                this.GroupFooter1});
            this.DetailReport.DataMember = "Rooms"; // Remap to ProjectQuoteReport DataSource
            this.DetailReport.Level = 0;
            this.DetailReport.Name = "DetailReport";

            // Detail1
            this.Detail1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.xrTable2});
            this.Detail1.HeightF = 68.83329F;
            this.Detail1.Name = "Detail1";

            // xrTable2
            this.xrTable2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrTable2.Name = "xrTable2";
            this.xrTable2.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
                this.xrTableRow2});
            this.xrTable2.SizeF = new System.Drawing.SizeF(830F, 25F);

            // xrTableRow2
            this.xrTableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.xrTableCell4,
                this.xrTableCell5,
                this.xrTableCell6});
            this.xrTableRow2.Name = "xrTableRow2";

            // xrTableCell4
            this.xrTableCell4.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[RoomName]")}); // Remap to ProjectQuoteReport FieldList
            this.xrTableCell4.Multiline = true;
            this.xrTableCell4.Name = "xrTableCell4";

            // xrTableCell5
            this.xrTableCell5.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[OptionalCost]")}); // Remap to ProjectQuoteReport FieldList
            this.xrTableCell5.Multiline = true;
            this.xrTableCell5.Name = "xrTableCell5";
            this.xrTableCell5.TextFormatString = "{0:C}";

            // xrTableCell6
            this.xrTableCell6.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Cost]")}); // Remap to ProjectQuoteReport FieldList
            this.xrTableCell6.Multiline = true;
            this.xrTableCell6.Name = "xrTableCell6";
            this.xrTableCell6.TextFormatString = "{0:C}";

            // GroupHeader2
            this.GroupHeader2.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.xrLabel2,
                this.xrLabel3,
                this.xrLabel17});
            this.GroupHeader2.HeightF = 27.08333F;
            this.GroupHeader2.Name = "GroupHeader2";

            // xrLabel2
            this.xrLabel2.Text = "Name";
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel2.Name = "xrLabel2";

            // xrLabel3
            this.xrLabel3.Text = "Optional Cost";
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(542.5001F, 0F);
            this.xrLabel3.Name = "xrLabel3";

            // xrLabel17
            this.xrLabel17.Text = "Cost";
            this.xrLabel17.LocationFloat = new DevExpress.Utils.PointFloat(702.0831F, 0F);
            this.xrLabel17.Name = "xrLabel17";

            // GroupFooter1
            this.GroupFooter1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.xrLabel24,
                this.xrLabel25,
                this.xrLine1,
                this.xrLabel19,
                this.xrLabel18});
            this.GroupFooter1.HeightF = 101.9168F;
            this.GroupFooter1.Name = "GroupFooter1";

            // xrLabel24
            this.xrLabel24.Text = "Total:";
            this.xrLabel24.LocationFloat = new DevExpress.Utils.PointFloat(615.6249F, 7.291667F);
            this.xrLabel24.Name = "xrLabel24";

            // xrLabel25
            this.xrLabel25.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Sum([Cost])")}); // Remap to ProjectQuoteReport FieldList
            this.xrLabel25.LocationFloat = new DevExpress.Utils.PointFloat(671.8749F, 7.291667F);
            this.xrLabel25.Name = "xrLabel25";
            this.xrLabel25.TextFormatString = "{0:C}";

            // xrLine1
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(2.916718F, 0F);
            this.xrLine1.Name = "xrLine1";

            // xrLabel19
            this.xrLabel19.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Discount]")}); // Remap to ProjectQuoteReport FieldList
            this.xrLabel19.LocationFloat = new DevExpress.Utils.PointFloat(676.0413F, 30.29169F);
            this.xrLabel19.Name = "xrLabel19";
            this.xrLabel19.TextFormatString = "{0:C}";

            // xrLabel18
            this.xrLabel18.Text = "Discount:";
            this.xrLabel18.LocationFloat = new DevExpress.Utils.PointFloat(585.4163F, 30.29169F);
            this.xrLabel18.Name = "xrLabel18";

            // PageFooter
            this.PageFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.xrLabel14});
            this.PageFooter.HeightF = 55.20833F;
            this.PageFooter.Name = "PageFooter";

            // xrLabel14
            this.xrLabel14.Text = "We look forward to working with you.";
            this.xrLabel14.LocationFloat = new DevExpress.Utils.PointFloat(0.4165649F, 0F);
            this.xrLabel14.Name = "xrLabel14";

            ((System.ComponentModel.ISupportInitialize)(this.xrTable2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
        }
    }
}