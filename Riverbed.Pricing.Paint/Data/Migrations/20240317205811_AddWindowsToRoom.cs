using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddWindowsToRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "AdditionalPrepTime",
                table: "Rooms",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IncludeWindows",
                table: "Rooms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfWindows",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalPrepTime",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "IncludeWindows",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "NumberOfWindows",
                table: "Rooms");
        }
    }
}
