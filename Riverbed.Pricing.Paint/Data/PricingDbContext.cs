using Microsoft.EntityFrameworkCore;
using Riverbed.Pricing.Paint.Entities;
using Riverbed.Pricing.Paint.Shared.Entities;
using Riverbed.Pricing.Paint.Shared.Entities.Paint;
using Riverbed.Pricing.Paint.Shared.Entities.Reporting;
using Riverbed.Pricing.Paint.Shared.Entities.StoredProc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riverbed.Pricing.Paint.Shared.Data
{
    public class PricingDbContext : DbContext
    {
        public PricingDbContext(DbContextOptions<PricingDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ProjectDetails>().HasNoKey();
            modelBuilder.Entity<RoomDetails>().HasNoKey();
            modelBuilder.Entity<CompanyCustomerDetails>().HasNoKey();
            modelBuilder.Entity<CompanyDetails>().HasNoKey();

            modelBuilder.Entity<RoomSurface>()
                .HasOne(rs => rs.SurfaceTypeLookup)
                .WithMany(st => st.RoomSurfaces)
                .HasForeignKey(rs => rs.SurfaceType)
                .HasPrincipalKey(st => st.SurfaceTypeId);

            modelBuilder.Entity<Room>()
           .HasMany(r => r.PaintableItems)
           .WithOne()
           .HasForeignKey(pi => pi.RoomId);

            modelBuilder.Entity<InvoiceStatus>().HasData(
                new InvoiceStatus { Id = 1, Name = "Not Ready" },
                new InvoiceStatus { Id = 2, Name = "Ready" },
                new InvoiceStatus { Id = 3, Name = "Sent" },
                new InvoiceStatus { Id = 4, Name = "Paid" },
                new InvoiceStatus { Id = 5, Name = "Cancelled" }
            );

            modelBuilder.Entity<WorkingStatus>().HasData(
                new WorkingStatus { Id = 1, Name = "Working on it" },
                new WorkingStatus { Id = 2, Name = "Stuck" },
                new WorkingStatus { Id = 3, Name = "Done" }
            );
        }

        public DbSet<Entities.Company> Companies { get; set; }
        public DbSet<Entities.CompanyDefaults> CompanyDefaults { get; set; }
        public DbSet<Entities.CompanySettings> CompanySettings { get; set; }
        public DbSet<Entities.PaintBrand> PaintBrands { get; set; }
        public DbSet<Entities.PaintQuality> PaintQualities { get; set; }
        public DbSet<Entities.PaintSheen> PaintSheens { get; set; }
        public DbSet<Entities.CompanyCustomer> CompanyCustomers { get; set; }

        public DbSet<Entities.ProjectData> Projects { get; set; }
        public DbSet<Entities.StatusCode> StatusCodes { get; set; }
        public DbSet<InvoiceStatus> InvoiceStatuses { get; set; }
        public DbSet<WorkingStatus> WorkingStatuses { get; set; }

        // Area Linked Tables
        public DbSet<Entities.ProjectArea> ProjectAreas { get; set; }
        public DbSet<Entities.AreaItem> AreaItems { get; set; }
        public DbSet<Entities.ItemType> ItemTypes { get; set; }
        public DbSet<Entities.ItemPaint> ItemPaints { get; set; }
        public DbSet<Entities.PaintType> PaintTypes { get; set; }
        public DbSet<Entities.CompanyPaintType> CompanyPaintTypes { get; set; }
        public DbSet<Entities.DifficultyLevel> DifficultyLevels { get; set; }
        public DbSet<Entities.PricingType> PricingType { get; set; }
        
        public DbSet<Entities.GlobalDefaults> GlobalDefaults { get; set; }

        // New Pricing Tables
        public DbSet<CustomerJob> CustomerJobs { get; set; }
        public DbSet<PaintExteriorRequirement> PaintExteriorRequirements { get; set; }
        public DbSet<PaintInteriorRequirement> PaintInteriorRequirements { get; set; }
        public DbSet<PricingExteriorDefault> PricingExteriorDefaults { get; set; }
        public DbSet<PricingInteriorDefault> PricingInteriorDefaults { get; set; }
        public DbSet<PricingRequestExterior> PricingRequestExteriors { get; set; }
        public DbSet<PricingRequestInterior> PricingRequestInteriors { get; set; }
        public DbSet<PricingResponseExterior> PricingResponseExteriors { get; set; }
        public DbSet<PricingResponseInterior> PricingResponseInteriors { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomGlobalDefaults> RoomGlobalDefaults { get; set; }
        public DbSet<Adjustment> Adjustments { get; set; }
        // Paintable Items
        public DbSet<PaintableItem> PaintableItems { get; set; }
        public DbSet<CompanyPaintableItem> CompanyPaintableItems { get; set; }

        // Stored Procedure data
        public DbSet<ProjectDetails> ProjectDetails { get; set; }
        public DbSet<RoomDetails> RoomDetails { get; set; }
        public DbSet<CompanyCustomerDetails> CompanyCustomerDetails { get; set; }
        public DbSet<CompanyDetails> CompanyDetails { get; set; }
        public DbSet<PaintableItemCategory> PaintableItemCategories { get; set; }
        public DbSet<ServiceTitanConnectionData> ServiceTitanConnectionDatas { get; set; }

        // Reporting
        public DbSet<CompanyHTMLReport> CompanyHTMLReports { get; set; }
        public DbSet<CompanyHTMLReportTemplate> CompanyHTMLReportsTemplate { get; set; }
        public DbSet<CompanyReportType> CompanyReportTypes { get; set; }

        // Files
        public DbSet<DataFile> DataFiles { get; set; }
        public DbSet<CompanyFile> CompanyFiles { get; set; }

        // Images
        public DbSet<ImageAsset> Images { get; set; }
        public DbSet<CompanyImage> CompanyImages { get; set; }

        // Email Logs
        public DbSet<EmailLog> EmailLogs { get; set; }

        // Paint Layer Types
        public DbSet<SurfaceTypeLookup> SurfaceTypes => Set<SurfaceTypeLookup>();
        public DbSet<RoomSurface> RoomSurfaces => Set<RoomSurface>();
        public DbSet<RoomSurfacePaintLayer> RoomSurfacePaintLayers => Set<RoomSurfacePaintLayer>();
    }
}
