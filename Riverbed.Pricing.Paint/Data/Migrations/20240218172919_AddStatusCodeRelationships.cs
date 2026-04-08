using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusCodeRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CustomersProject_StatusCodeId",
                table: "CustomersProject",
                column: "StatusCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomersProject_StatusCodes_StatusCodeId",
                table: "CustomersProject",
                column: "StatusCodeId",
                principalTable: "StatusCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomersProject_StatusCodes_StatusCodeId",
                table: "CustomersProject");

            migrationBuilder.DropIndex(
                name: "IX_CustomersProject_StatusCodeId",
                table: "CustomersProject");
        }
    }
}
