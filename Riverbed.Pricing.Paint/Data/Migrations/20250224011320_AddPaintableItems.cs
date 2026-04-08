using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddPaintableItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaintableItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "money", nullable: false),
                    PaintTypeId = table.Column<int>(type: "int", nullable: false),
                    PricingTypeId = table.Column<int>(type: "int", nullable: false),
                    SquareFootage = table.Column<double>(type: "float", nullable: false),
                    BaseTime = table.Column<double>(type: "float", nullable: false),
                    AdditionalTime = table.Column<double>(type: "float", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaintableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaintableItems_PaintTypes_PaintTypeId",
                        column: x => x.PaintTypeId,
                        principalTable: "PaintTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaintableItems_PricingType_PricingTypeId",
                        column: x => x.PricingTypeId,
                        principalTable: "PricingType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaintableItems_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaintableItems_PaintTypeId",
                table: "PaintableItems",
                column: "PaintTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PaintableItems_PricingTypeId",
                table: "PaintableItems",
                column: "PricingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PaintableItems_RoomId",
                table: "PaintableItems",
                column: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaintableItems");
        }
    }
}
