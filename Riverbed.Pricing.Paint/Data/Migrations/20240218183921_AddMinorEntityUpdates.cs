using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddMinorEntityUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaintBrandId",
                table: "PaintTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PaintTypes_PaintBrandId",
                table: "PaintTypes",
                column: "PaintBrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaintTypes_PaintBrands_PaintBrandId",
                table: "PaintTypes",
                column: "PaintBrandId",
                principalTable: "PaintBrands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaintTypes_PaintBrands_PaintBrandId",
                table: "PaintTypes");

            migrationBuilder.DropIndex(
                name: "IX_PaintTypes_PaintBrandId",
                table: "PaintTypes");

            migrationBuilder.DropColumn(
                name: "PaintBrandId",
                table: "PaintTypes");
        }
    }
}
