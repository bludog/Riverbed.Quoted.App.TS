using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddAllFieldsForPricingInteriorDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyGuid",
                table: "PricingRequestInteriors",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<float>(
                name: "BaseboardRatePerLinearFoot",
                table: "PricingInteriorDefaults",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "CeilingRatePerSquareFoot",
                table: "PricingInteriorDefaults",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "CrownMoldingRatePerLinearFoot",
                table: "PricingInteriorDefaults",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "DoorRateEach",
                table: "PricingInteriorDefaults",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "PaintCoats",
                table: "PricingInteriorDefaults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaintCoveragePerGallon",
                table: "PricingInteriorDefaults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "WallRatePerSquareFoot",
                table: "PricingInteriorDefaults",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyGuid",
                table: "PricingRequestInteriors");

            migrationBuilder.DropColumn(
                name: "BaseboardRatePerLinearFoot",
                table: "PricingInteriorDefaults");

            migrationBuilder.DropColumn(
                name: "CeilingRatePerSquareFoot",
                table: "PricingInteriorDefaults");

            migrationBuilder.DropColumn(
                name: "CrownMoldingRatePerLinearFoot",
                table: "PricingInteriorDefaults");

            migrationBuilder.DropColumn(
                name: "DoorRateEach",
                table: "PricingInteriorDefaults");

            migrationBuilder.DropColumn(
                name: "PaintCoats",
                table: "PricingInteriorDefaults");

            migrationBuilder.DropColumn(
                name: "PaintCoveragePerGallon",
                table: "PricingInteriorDefaults");

            migrationBuilder.DropColumn(
                name: "WallRatePerSquareFoot",
                table: "PricingInteriorDefaults");
        }
    }
}
