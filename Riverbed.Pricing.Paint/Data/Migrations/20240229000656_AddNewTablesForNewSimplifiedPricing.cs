using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTablesForNewSimplifiedPricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaintExteriorRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SidingPaintGallons = table.Column<double>(type: "float", nullable: false),
                    BoxingPaintGallons = table.Column<double>(type: "float", nullable: false),
                    DoorsPaintGallons = table.Column<double>(type: "float", nullable: false),
                    GarageDoorsPaintGallons = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaintExteriorRequirements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaintInteriorRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WallsPaint = table.Column<double>(type: "float", nullable: false),
                    CeilingPaint = table.Column<double>(type: "float", nullable: false),
                    DoorsPaint = table.Column<double>(type: "float", nullable: false),
                    BaseboardsPaint = table.Column<double>(type: "float", nullable: false),
                    CrownMoldingsPaint = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaintInteriorRequirements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PricingExteriorDefaults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExteriorDoorRate = table.Column<double>(type: "float", nullable: false),
                    SingleGarageDoorRate = table.Column<double>(type: "float", nullable: false),
                    DoubleGarageDoorRate = table.Column<double>(type: "float", nullable: false),
                    BoxingRatePerLinearFoot = table.Column<double>(type: "float", nullable: false),
                    SidingRatePerSquareFoot = table.Column<double>(type: "float", nullable: false),
                    ChimneyRateFlat = table.Column<double>(type: "float", nullable: false),
                    PaintCoveragePerGallon = table.Column<int>(type: "int", nullable: false),
                    PaintCoats = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingExteriorDefaults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PricingInteriorDefaults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingInteriorDefaults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PricingRequestExteriors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Length = table.Column<double>(type: "float", nullable: false),
                    Width = table.Column<double>(type: "float", nullable: false),
                    HeightToRoofBase = table.Column<double>(type: "float", nullable: false),
                    ExteriorDoorCount = table.Column<int>(type: "int", nullable: false),
                    SingleGarageDoors = table.Column<int>(type: "int", nullable: false),
                    DoubleGarageDoors = table.Column<int>(type: "int", nullable: false),
                    IncludeBoxing = table.Column<bool>(type: "bit", nullable: false),
                    IncludeSiding = table.Column<bool>(type: "bit", nullable: false),
                    IncludeChimney = table.Column<bool>(type: "bit", nullable: false),
                    PaintQualityId = table.Column<int>(type: "int", nullable: false),
                    PaintQuality = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingRequestExteriors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PricingRequestInteriors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingRequestInteriors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PricingResponseInteriors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingResponseInteriors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PricingResponseExteriors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalCost = table.Column<double>(type: "float", nullable: false),
                    DoorsCost = table.Column<double>(type: "float", nullable: false),
                    GarageDoorsCost = table.Column<double>(type: "float", nullable: false),
                    BoxingCost = table.Column<double>(type: "float", nullable: false),
                    SidingCost = table.Column<double>(type: "float", nullable: false),
                    ChimneyCost = table.Column<double>(type: "float", nullable: false),
                    TotalMaterialCost = table.Column<double>(type: "float", nullable: false),
                    PricingRequestExteriorId = table.Column<int>(type: "int", nullable: false),
                    PaintExteriorRequirementsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingResponseExteriors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PricingResponseExteriors_PaintExteriorRequirements_PaintExteriorRequirementsId",
                        column: x => x.PaintExteriorRequirementsId,
                        principalTable: "PaintExteriorRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyCustomerId = table.Column<int>(type: "int", nullable: true),
                    PricingRequestInteriorId = table.Column<int>(type: "int", nullable: true),
                    PricingRequestExteriorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerJobs_CompanyCustomers_CompanyCustomerId",
                        column: x => x.CompanyCustomerId,
                        principalTable: "CompanyCustomers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerJobs_PricingRequestExteriors_PricingRequestExteriorId",
                        column: x => x.PricingRequestExteriorId,
                        principalTable: "PricingRequestExteriors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerJobs_PricingRequestInteriors_PricingRequestInteriorId",
                        column: x => x.PricingRequestInteriorId,
                        principalTable: "PricingRequestInteriors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Length = table.Column<double>(type: "float", nullable: false),
                    Width = table.Column<double>(type: "float", nullable: false),
                    Height = table.Column<double>(type: "float", nullable: false),
                    NumberOfDoors = table.Column<int>(type: "int", nullable: false),
                    IncludeCeiling = table.Column<bool>(type: "bit", nullable: false),
                    IncludeBaseboards = table.Column<bool>(type: "bit", nullable: false),
                    IncludeCrownMoldings = table.Column<bool>(type: "bit", nullable: false),
                    PaintQualityId = table.Column<int>(type: "int", nullable: false),
                    PaintQuality = table.Column<int>(type: "int", nullable: false),
                    PricingRequestInteriorId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Room_PricingRequestInteriors_PricingRequestInteriorId",
                        column: x => x.PricingRequestInteriorId,
                        principalTable: "PricingRequestInteriors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomPricingDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalCost = table.Column<double>(type: "float", nullable: false),
                    WallsCost = table.Column<double>(type: "float", nullable: false),
                    CeilingCost = table.Column<double>(type: "float", nullable: false),
                    DoorsCost = table.Column<double>(type: "float", nullable: false),
                    BaseboardsCost = table.Column<double>(type: "float", nullable: false),
                    CrownMoldingsCost = table.Column<double>(type: "float", nullable: false),
                    PricingRequestInteriorId = table.Column<int>(type: "int", nullable: false),
                    PaintRequirementsId = table.Column<int>(type: "int", nullable: false),
                    PaintInteriorRequirementsId = table.Column<int>(type: "int", nullable: false),
                    PricingResponseInteriorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomPricingDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomPricingDetail_PaintInteriorRequirements_PaintInteriorRequirementsId",
                        column: x => x.PaintInteriorRequirementsId,
                        principalTable: "PaintInteriorRequirements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomPricingDetail_PricingResponseInteriors_PricingResponseInteriorId",
                        column: x => x.PricingResponseInteriorId,
                        principalTable: "PricingResponseInteriors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerJobs_CompanyCustomerId",
                table: "CustomerJobs",
                column: "CompanyCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerJobs_PricingRequestExteriorId",
                table: "CustomerJobs",
                column: "PricingRequestExteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerJobs_PricingRequestInteriorId",
                table: "CustomerJobs",
                column: "PricingRequestInteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_PricingResponseExteriors_PaintExteriorRequirementsId",
                table: "PricingResponseExteriors",
                column: "PaintExteriorRequirementsId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_PricingRequestInteriorId",
                table: "Room",
                column: "PricingRequestInteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomPricingDetail_PaintInteriorRequirementsId",
                table: "RoomPricingDetail",
                column: "PaintInteriorRequirementsId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomPricingDetail_PricingResponseInteriorId",
                table: "RoomPricingDetail",
                column: "PricingResponseInteriorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerJobs");

            migrationBuilder.DropTable(
                name: "PricingExteriorDefaults");

            migrationBuilder.DropTable(
                name: "PricingInteriorDefaults");

            migrationBuilder.DropTable(
                name: "PricingResponseExteriors");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "RoomPricingDetail");

            migrationBuilder.DropTable(
                name: "PricingRequestExteriors");

            migrationBuilder.DropTable(
                name: "PaintExteriorRequirements");

            migrationBuilder.DropTable(
                name: "PricingRequestInteriors");

            migrationBuilder.DropTable(
                name: "PaintInteriorRequirements");

            migrationBuilder.DropTable(
                name: "PricingResponseInteriors");
        }
    }
}
