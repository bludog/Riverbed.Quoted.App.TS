using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultPaintTypesForItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaintTypeBaseboardsId",
                table: "CompanyDefaults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaintTypeCeilingsId",
                table: "CompanyDefaults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaintTypeTrimDoorsId",
                table: "CompanyDefaults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaintTypeWallsId",
                table: "CompanyDefaults",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaintTypeBaseboardsId",
                table: "CompanyDefaults");

            migrationBuilder.DropColumn(
                name: "PaintTypeCeilingsId",
                table: "CompanyDefaults");

            migrationBuilder.DropColumn(
                name: "PaintTypeTrimDoorsId",
                table: "CompanyDefaults");

            migrationBuilder.DropColumn(
                name: "PaintTypeWallsId",
                table: "CompanyDefaults");
        }
    }
}
