using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyPaintTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyPaintTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaintTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CoverageOneCoatSqFt = table.Column<int>(type: "int", nullable: false),
                    CoverageTwoCoatsSqFt = table.Column<int>(type: "int", nullable: false),
                    PricePerGallon = table.Column<float>(type: "real", nullable: false),
                    PaintSheenId = table.Column<int>(type: "int", nullable: false),
                    PaintBrandId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyPaintTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyPaintTypes_PaintBrands_PaintBrandId",
                        column: x => x.PaintBrandId,
                        principalTable: "PaintBrands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyPaintTypes_PaintSheens_PaintSheenId",
                        column: x => x.PaintSheenId,
                        principalTable: "PaintSheens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPaintTypes_PaintBrandId",
                table: "CompanyPaintTypes",
                column: "PaintBrandId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPaintTypes_PaintSheenId",
                table: "CompanyPaintTypes",
                column: "PaintSheenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyPaintTypes");
        }
    }
}
