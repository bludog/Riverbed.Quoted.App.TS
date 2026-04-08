using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedReferencesFromCompDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaintBrandId",
                table: "CompanyDefaults");

            migrationBuilder.DropColumn(
                name: "PaintQualityId",
                table: "CompanyDefaults");

            migrationBuilder.DropColumn(
                name: "PaintSheenId",
                table: "CompanyDefaults");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaintBrandId",
                table: "CompanyDefaults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaintQualityId",
                table: "CompanyDefaults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaintSheenId",
                table: "CompanyDefaults",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
