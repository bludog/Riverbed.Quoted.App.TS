using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddProjAreaRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DifficultyId",
                table: "AreaItems",
                newName: "PricingTypeId");

            migrationBuilder.RenameColumn(
                name: "AreaId",
                table: "AreaItems",
                newName: "DifficultyLevelId");

            migrationBuilder.AddColumn<int>(
                name: "ItemPaintId",
                table: "AreaItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectAreaId",
                table: "AreaItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AreaItems_DifficultyLevelId",
                table: "AreaItems",
                column: "DifficultyLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_AreaItems_ItemPaintId",
                table: "AreaItems",
                column: "ItemPaintId");

            migrationBuilder.CreateIndex(
                name: "IX_AreaItems_ItemTypeId",
                table: "AreaItems",
                column: "ItemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AreaItems_PricingTypeId",
                table: "AreaItems",
                column: "PricingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AreaItems_ProjectAreaId",
                table: "AreaItems",
                column: "ProjectAreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_AreaItems_DifficultyLevels_DifficultyLevelId",
                table: "AreaItems",
                column: "DifficultyLevelId",
                principalTable: "DifficultyLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AreaItems_ItemPaints_ItemPaintId",
                table: "AreaItems",
                column: "ItemPaintId",
                principalTable: "ItemPaints",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AreaItems_ItemTypes_ItemTypeId",
                table: "AreaItems",
                column: "ItemTypeId",
                principalTable: "ItemTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AreaItems_PricingType_PricingTypeId",
                table: "AreaItems",
                column: "PricingTypeId",
                principalTable: "PricingType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AreaItems_ProjectAreas_ProjectAreaId",
                table: "AreaItems",
                column: "ProjectAreaId",
                principalTable: "ProjectAreas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AreaItems_DifficultyLevels_DifficultyLevelId",
                table: "AreaItems");

            migrationBuilder.DropForeignKey(
                name: "FK_AreaItems_ItemPaints_ItemPaintId",
                table: "AreaItems");

            migrationBuilder.DropForeignKey(
                name: "FK_AreaItems_ItemTypes_ItemTypeId",
                table: "AreaItems");

            migrationBuilder.DropForeignKey(
                name: "FK_AreaItems_PricingType_PricingTypeId",
                table: "AreaItems");

            migrationBuilder.DropForeignKey(
                name: "FK_AreaItems_ProjectAreas_ProjectAreaId",
                table: "AreaItems");

            migrationBuilder.DropIndex(
                name: "IX_AreaItems_DifficultyLevelId",
                table: "AreaItems");

            migrationBuilder.DropIndex(
                name: "IX_AreaItems_ItemPaintId",
                table: "AreaItems");

            migrationBuilder.DropIndex(
                name: "IX_AreaItems_ItemTypeId",
                table: "AreaItems");

            migrationBuilder.DropIndex(
                name: "IX_AreaItems_PricingTypeId",
                table: "AreaItems");

            migrationBuilder.DropIndex(
                name: "IX_AreaItems_ProjectAreaId",
                table: "AreaItems");

            migrationBuilder.DropColumn(
                name: "ItemPaintId",
                table: "AreaItems");

            migrationBuilder.DropColumn(
                name: "ProjectAreaId",
                table: "AreaItems");

            migrationBuilder.RenameColumn(
                name: "PricingTypeId",
                table: "AreaItems",
                newName: "DifficultyId");

            migrationBuilder.RenameColumn(
                name: "DifficultyLevelId",
                table: "AreaItems",
                newName: "AreaId");
        }
    }
}
