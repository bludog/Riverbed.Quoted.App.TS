using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaintQualityId",
                table: "ItemPaints");

            migrationBuilder.DropColumn(
                name: "PaintSheenId",
                table: "ItemPaints");

            migrationBuilder.CreateIndex(
                name: "IX_PaintTypes_PaintSheenId",
                table: "PaintTypes",
                column: "PaintSheenId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPaints_PaintTypeId",
                table: "ItemPaints",
                column: "PaintTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemPaints_PaintTypes_PaintTypeId",
                table: "ItemPaints",
                column: "PaintTypeId",
                principalTable: "PaintTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaintTypes_PaintSheens_PaintSheenId",
                table: "PaintTypes",
                column: "PaintSheenId",
                principalTable: "PaintSheens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemPaints_PaintTypes_PaintTypeId",
                table: "ItemPaints");

            migrationBuilder.DropForeignKey(
                name: "FK_PaintTypes_PaintSheens_PaintSheenId",
                table: "PaintTypes");

            migrationBuilder.DropIndex(
                name: "IX_PaintTypes_PaintSheenId",
                table: "PaintTypes");

            migrationBuilder.DropIndex(
                name: "IX_ItemPaints_PaintTypeId",
                table: "ItemPaints");

            migrationBuilder.AddColumn<int>(
                name: "PaintQualityId",
                table: "ItemPaints",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaintSheenId",
                table: "ItemPaints",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
