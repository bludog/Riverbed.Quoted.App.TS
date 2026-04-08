using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomImageId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoomImageId",
                table: "Rooms",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomImageId",
                table: "Rooms",
                column: "RoomImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_DataFiles_RoomImageId",
                table: "Rooms",
                column: "RoomImageId",
                principalTable: "DataFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_DataFiles_RoomImageId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_RoomImageId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "RoomImageId",
                table: "Rooms");
        }
    }
}
