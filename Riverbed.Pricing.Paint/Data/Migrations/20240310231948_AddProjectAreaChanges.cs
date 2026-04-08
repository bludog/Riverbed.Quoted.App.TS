using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectAreaChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "AreaTotal",
                table: "ProjectAreas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "LaborCost",
                table: "ProjectAreas",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "MaterialCost",
                table: "ProjectAreas",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AreaTotal",
                table: "ProjectAreas");

            migrationBuilder.DropColumn(
                name: "LaborCost",
                table: "ProjectAreas");

            migrationBuilder.DropColumn(
                name: "MaterialCost",
                table: "ProjectAreas");
        }
    }
}
