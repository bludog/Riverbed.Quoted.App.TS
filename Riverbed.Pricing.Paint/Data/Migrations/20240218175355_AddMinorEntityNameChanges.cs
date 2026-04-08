using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddMinorEntityNameChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaintTypeNameId",
                table: "PaintTypes");

            migrationBuilder.AddColumn<string>(
                name: "PaintTypeName",
                table: "PaintTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaintTypeName",
                table: "PaintTypes");

            migrationBuilder.AddColumn<int>(
                name: "PaintTypeNameId",
                table: "PaintTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
