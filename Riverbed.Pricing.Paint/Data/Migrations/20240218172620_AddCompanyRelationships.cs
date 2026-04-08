using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "CustomersProject",
                newName: "CompanyCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAreas_CustomerProjectId",
                table: "ProjectAreas",
                column: "CustomerProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomersProject_CompanyCustomerId",
                table: "CustomersProject",
                column: "CompanyCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyCustomers_CompanyId",
                table: "CompanyCustomers",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyCustomers_Companies_CompanyId",
                table: "CompanyCustomers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomersProject_CompanyCustomers_CompanyCustomerId",
                table: "CustomersProject",
                column: "CompanyCustomerId",
                principalTable: "CompanyCustomers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectAreas_CustomersProject_CustomerProjectId",
                table: "ProjectAreas",
                column: "CustomerProjectId",
                principalTable: "CustomersProject",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyCustomers_Companies_CompanyId",
                table: "CompanyCustomers");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomersProject_CompanyCustomers_CompanyCustomerId",
                table: "CustomersProject");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectAreas_CustomersProject_CustomerProjectId",
                table: "ProjectAreas");

            migrationBuilder.DropIndex(
                name: "IX_ProjectAreas_CustomerProjectId",
                table: "ProjectAreas");

            migrationBuilder.DropIndex(
                name: "IX_CustomersProject_CompanyCustomerId",
                table: "CustomersProject");

            migrationBuilder.DropIndex(
                name: "IX_CompanyCustomers_CompanyId",
                table: "CompanyCustomers");

            migrationBuilder.RenameColumn(
                name: "CompanyCustomerId",
                table: "CustomersProject",
                newName: "CustomerId");
        }
    }
}
