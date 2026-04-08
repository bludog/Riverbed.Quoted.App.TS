using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class UpdatezCustomerProjectInDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectAreas_CustomersProjects_CustomerProjectId",
                table: "ProjectAreas");

            migrationBuilder.DropIndex(
                name: "IX_ProjectAreas_CustomerProjectId",
                table: "ProjectAreas");

            migrationBuilder.AddColumn<int>(
                name: "CustomersProjectId",
                table: "ProjectAreas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAreas_CustomersProjectId",
                table: "ProjectAreas",
                column: "CustomersProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectAreas_CustomersProjects_CustomersProjectId",
                table: "ProjectAreas",
                column: "CustomersProjectId",
                principalTable: "CustomersProjects",
                principalColumn: "Id");
        }
    }
}
