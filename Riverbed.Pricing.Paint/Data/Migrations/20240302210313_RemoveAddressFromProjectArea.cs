using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAddressFromProjectArea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address1",
                table: "ProjectAreas");

            migrationBuilder.DropColumn(
                name: "Address2",
                table: "ProjectAreas");

            migrationBuilder.DropColumn(
                name: "City",
                table: "ProjectAreas");

            migrationBuilder.DropColumn(
                name: "StateCode",
                table: "ProjectAreas");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "ProjectAreas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address1",
                table: "ProjectAreas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address2",
                table: "ProjectAreas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "ProjectAreas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StateCode",
                table: "ProjectAreas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "ProjectAreas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
