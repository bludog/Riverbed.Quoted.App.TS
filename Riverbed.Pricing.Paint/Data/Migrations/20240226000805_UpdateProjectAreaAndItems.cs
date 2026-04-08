using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProjectAreaAndItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AreaItems_ItemPaints_ItemPaintId",
                table: "AreaItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemPaints_AreaItems_AreaItemId",
                table: "ItemPaints");

            migrationBuilder.DropIndex(
                name: "IX_ItemPaints_AreaItemId",
                table: "ItemPaints");

            migrationBuilder.DropIndex(
                name: "IX_AreaItems_ItemPaintId",
                table: "AreaItems");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "ItemPaints");

            migrationBuilder.RenameColumn(
                name: "ItemPaintId",
                table: "AreaItems",
                newName: "PaintTypeId");

            migrationBuilder.AddColumn<bool>(
                name: "PrimeWalls",
                table: "ProjectAreas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "AreaItemId",
                table: "ItemPaints",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<float>(
                name: "LaborCost",
                table: "AreaItems",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "MaterialCost",
                table: "AreaItems",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "MaterialNeeded",
                table: "AreaItems",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "TimeNeeded",
                table: "AreaItems",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "TotalArea",
                table: "AreaItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ItemPaints_AreaItemId",
                table: "ItemPaints",
                column: "AreaItemId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemPaints_AreaItems_AreaItemId",
                table: "ItemPaints",
                column: "AreaItemId",
                principalTable: "AreaItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemPaints_AreaItems_AreaItemId",
                table: "ItemPaints");

            migrationBuilder.DropIndex(
                name: "IX_ItemPaints_AreaItemId",
                table: "ItemPaints");

            migrationBuilder.DropColumn(
                name: "PrimeWalls",
                table: "ProjectAreas");

            migrationBuilder.DropColumn(
                name: "LaborCost",
                table: "AreaItems");

            migrationBuilder.DropColumn(
                name: "MaterialCost",
                table: "AreaItems");

            migrationBuilder.DropColumn(
                name: "MaterialNeeded",
                table: "AreaItems");

            migrationBuilder.DropColumn(
                name: "TimeNeeded",
                table: "AreaItems");

            migrationBuilder.DropColumn(
                name: "TotalArea",
                table: "AreaItems");

            migrationBuilder.RenameColumn(
                name: "PaintTypeId",
                table: "AreaItems",
                newName: "ItemPaintId");

            migrationBuilder.AlterColumn<int>(
                name: "AreaItemId",
                table: "ItemPaints",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "ItemPaints",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ItemPaints_AreaItemId",
                table: "ItemPaints",
                column: "AreaItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AreaItems_ItemPaintId",
                table: "AreaItems",
                column: "ItemPaintId");

            migrationBuilder.AddForeignKey(
                name: "FK_AreaItems_ItemPaints_ItemPaintId",
                table: "AreaItems",
                column: "ItemPaintId",
                principalTable: "ItemPaints",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemPaints_AreaItems_AreaItemId",
                table: "ItemPaints",
                column: "AreaItemId",
                principalTable: "AreaItems",
                principalColumn: "Id");
        }
    }
}
