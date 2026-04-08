using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddPricingRoomDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_PricingRequestInteriors_PricingRequestInteriorId",
                table: "Rooms");

            migrationBuilder.AlterColumn<int>(
                name: "PricingRequestInteriorId",
                table: "Rooms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ProjectAreaId",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RoomGlobalDefaults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Length = table.Column<double>(type: "float", nullable: false),
                    Width = table.Column<double>(type: "float", nullable: false),
                    Height = table.Column<double>(type: "float", nullable: false),
                    NumberOfDoors = table.Column<int>(type: "int", nullable: false),
                    IncludeCeiling = table.Column<bool>(type: "bit", nullable: false),
                    IncludeBaseboards = table.Column<bool>(type: "bit", nullable: false),
                    IncludeCrownMoldings = table.Column<bool>(type: "bit", nullable: false),
                    PaintQualityId = table.Column<int>(type: "int", nullable: false),
                    PaintQuality = table.Column<int>(type: "int", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomGlobalDefaults", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_ProjectAreaId",
                table: "Rooms",
                column: "ProjectAreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_PricingRequestInteriors_PricingRequestInteriorId",
                table: "Rooms",
                column: "PricingRequestInteriorId",
                principalTable: "PricingRequestInteriors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_ProjectAreas_ProjectAreaId",
                table: "Rooms",
                column: "ProjectAreaId",
                principalTable: "ProjectAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_PricingRequestInteriors_PricingRequestInteriorId",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_ProjectAreas_ProjectAreaId",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "RoomGlobalDefaults");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_ProjectAreaId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "ProjectAreaId",
                table: "Rooms");

            migrationBuilder.AlterColumn<int>(
                name: "PricingRequestInteriorId",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_PricingRequestInteriors_PricingRequestInteriorId",
                table: "Rooms",
                column: "PricingRequestInteriorId",
                principalTable: "PricingRequestInteriors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
