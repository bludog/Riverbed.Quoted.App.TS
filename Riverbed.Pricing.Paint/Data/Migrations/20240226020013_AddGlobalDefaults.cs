using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddGlobalDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GlobalDefaults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HourlyRate = table.Column<float>(type: "real", nullable: false),
                    TimePerSquareFootWall = table.Column<float>(type: "real", nullable: false),
                    TimePerSquareFootCeiling = table.Column<float>(type: "real", nullable: false),
                    TimePerSquareFootBaseboard = table.Column<float>(type: "real", nullable: false),
                    TimePerSquareFootDoor = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalDefaults", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlobalDefaults");
        }
    }
}
