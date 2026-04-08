using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTableForRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Room_PricingRequestInteriors_PricingRequestInteriorId",
                table: "Room");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Room",
                table: "Room");

            migrationBuilder.RenameTable(
                name: "Room",
                newName: "Rooms");

            migrationBuilder.RenameIndex(
                name: "IX_Room_PricingRequestInteriorId",
                table: "Rooms",
                newName: "IX_Rooms_PricingRequestInteriorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_PricingRequestInteriors_PricingRequestInteriorId",
                table: "Rooms",
                column: "PricingRequestInteriorId",
                principalTable: "PricingRequestInteriors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_PricingRequestInteriors_PricingRequestInteriorId",
                table: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms");

            migrationBuilder.RenameTable(
                name: "Rooms",
                newName: "Room");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_PricingRequestInteriorId",
                table: "Room",
                newName: "IX_Room_PricingRequestInteriorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Room",
                table: "Room",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Room_PricingRequestInteriors_PricingRequestInteriorId",
                table: "Room",
                column: "PricingRequestInteriorId",
                principalTable: "PricingRequestInteriors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
