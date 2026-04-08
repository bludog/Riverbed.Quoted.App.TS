using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddPricingAreaChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Width",
                table: "RoomGlobalDefaults",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "Length",
                table: "RoomGlobalDefaults",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "Height",
                table: "RoomGlobalDefaults",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<bool>(
                name: "IncludeBaseboards",
                table: "ProjectAreas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IncludeCeiling",
                table: "ProjectAreas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IncludeCrownMoldings",
                table: "ProjectAreas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfDoors",
                table: "ProjectAreas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaintQuality",
                table: "ProjectAreas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaintQualityId",
                table: "ProjectAreas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncludeBaseboards",
                table: "ProjectAreas");

            migrationBuilder.DropColumn(
                name: "IncludeCeiling",
                table: "ProjectAreas");

            migrationBuilder.DropColumn(
                name: "IncludeCrownMoldings",
                table: "ProjectAreas");

            migrationBuilder.DropColumn(
                name: "NumberOfDoors",
                table: "ProjectAreas");

            migrationBuilder.DropColumn(
                name: "PaintQuality",
                table: "ProjectAreas");

            migrationBuilder.DropColumn(
                name: "PaintQualityId",
                table: "ProjectAreas");

            migrationBuilder.AlterColumn<double>(
                name: "Width",
                table: "RoomGlobalDefaults",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "Length",
                table: "RoomGlobalDefaults",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "Height",
                table: "RoomGlobalDefaults",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
