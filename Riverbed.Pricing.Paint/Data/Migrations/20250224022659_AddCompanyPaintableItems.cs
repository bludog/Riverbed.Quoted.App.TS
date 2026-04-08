using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyPaintableItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Coats",
                table: "PaintableItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CompanyPaintableItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaintTypeId = table.Column<int>(type: "int", nullable: false),
                    PricingTypeId = table.Column<int>(type: "int", nullable: false),
                    SquareFootage = table.Column<double>(type: "float", nullable: false),
                    BaseTime = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyPaintableItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyPaintableItems_PaintTypes_PaintTypeId",
                        column: x => x.PaintTypeId,
                        principalTable: "PaintTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyPaintableItems_PricingType_PricingTypeId",
                        column: x => x.PricingTypeId,
                        principalTable: "PricingType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPaintableItems_PaintTypeId",
                table: "CompanyPaintableItems",
                column: "PaintTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPaintableItems_PricingTypeId",
                table: "CompanyPaintableItems",
                column: "PricingTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyPaintableItems");

            migrationBuilder.DropColumn(
                name: "Coats",
                table: "PaintableItems");
        }
    }
}
