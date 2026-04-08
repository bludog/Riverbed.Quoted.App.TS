using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreFieldsToTheRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IncludeDoors",
                table: "Rooms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IncludeWalls",
                table: "Rooms",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncludeDoors",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "IncludeWalls",
                table: "Rooms");
        }
    }
}
