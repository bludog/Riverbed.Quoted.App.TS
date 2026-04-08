using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class RemovePaintableItemDependencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyPaintableItems_PaintTypes_PaintTypeId",
                table: "CompanyPaintableItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyPaintableItems_PricingType_PricingTypeId",
                table: "CompanyPaintableItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PaintableItems_PaintTypes_PaintTypeId",
                table: "PaintableItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PaintableItems_PricingType_PricingTypeId",
                table: "PaintableItems");

            migrationBuilder.DropIndex(
                name: "IX_PaintableItems_PaintTypeId",
                table: "PaintableItems");

            migrationBuilder.DropIndex(
                name: "IX_PaintableItems_PricingTypeId",
                table: "PaintableItems");

            migrationBuilder.DropIndex(
                name: "IX_CompanyPaintableItems_PaintTypeId",
                table: "CompanyPaintableItems");

            migrationBuilder.DropIndex(
                name: "IX_CompanyPaintableItems_PricingTypeId",
                table: "CompanyPaintableItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PaintableItems_PaintTypeId",
                table: "PaintableItems",
                column: "PaintTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PaintableItems_PricingTypeId",
                table: "PaintableItems",
                column: "PricingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPaintableItems_PaintTypeId",
                table: "CompanyPaintableItems",
                column: "PaintTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPaintableItems_PricingTypeId",
                table: "CompanyPaintableItems",
                column: "PricingTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyPaintableItems_PaintTypes_PaintTypeId",
                table: "CompanyPaintableItems",
                column: "PaintTypeId",
                principalTable: "PaintTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyPaintableItems_PricingType_PricingTypeId",
                table: "CompanyPaintableItems",
                column: "PricingTypeId",
                principalTable: "PricingType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaintableItems_PaintTypes_PaintTypeId",
                table: "PaintableItems",
                column: "PaintTypeId",
                principalTable: "PaintTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaintableItems_PricingType_PricingTypeId",
                table: "PaintableItems",
                column: "PricingTypeId",
                principalTable: "PricingType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
