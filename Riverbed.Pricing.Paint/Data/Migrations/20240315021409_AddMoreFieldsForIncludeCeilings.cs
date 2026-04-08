using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreFieldsForIncludeCeilings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IncludeCeiling",
                table: "Rooms",
                newName: "IncludeCeilings");

            migrationBuilder.RenameColumn(
                name: "IncludeCeiling",
                table: "RoomGlobalDefaults",
                newName: "IncludeCeilings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IncludeCeilings",
                table: "Rooms",
                newName: "IncludeCeiling");

            migrationBuilder.RenameColumn(
                name: "IncludeCeilings",
                table: "RoomGlobalDefaults",
                newName: "IncludeCeiling");
        }
    }
}
