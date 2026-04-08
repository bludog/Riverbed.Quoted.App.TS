using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddProfitToTheRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Profit",
                table: "Rooms",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "RoomPricingDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Profit",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "RoomPricingDetail");
        }
    }
}
