using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddReferenceFromAreaToProjectData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectDataId",
                table: "ProjectAreas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAreas_ProjectDataId",
                table: "ProjectAreas",
                column: "ProjectDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectAreas_Projects_ProjectDataId",
                table: "ProjectAreas",
                column: "ProjectDataId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectAreas_Projects_ProjectDataId",
                table: "ProjectAreas");

            migrationBuilder.DropIndex(
                name: "IX_ProjectAreas_ProjectDataId",
                table: "ProjectAreas");

            migrationBuilder.DropColumn(
                name: "ProjectDataId",
                table: "ProjectAreas");
        }
    }
}
