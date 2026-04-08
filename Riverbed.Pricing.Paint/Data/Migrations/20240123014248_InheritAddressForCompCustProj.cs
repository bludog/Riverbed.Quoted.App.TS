using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class InheritAddressForCompCustProj : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Companies_Addresses_AddressId",
            //    table: "Companies");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_CompanyCustomers_Addresses_AddressId",
            //    table: "CompanyCustomers");

            //migrationBuilder.DropTable(
            //    name: "Addresses");

            //migrationBuilder.DropIndex(
            //    name: "IX_CompanyCustomers_AddressId",
            //    table: "CompanyCustomers");

            //migrationBuilder.DropIndex(
            //    name: "IX_Companies_AddressId",
            //    table: "Companies");

            //migrationBuilder.DropColumn(
            //    name: "AddressId",
            //    table: "CompanyCustomers");

            //migrationBuilder.DropColumn(
            //    name: "AddressId",
            //    table: "Companies");

            migrationBuilder.AddColumn<string>(
                name: "Address1",
                table: "ProjectAreas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address2",
                table: "ProjectAreas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "ProjectAreas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StateCode",
                table: "ProjectAreas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "ProjectAreas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address1",
                table: "CompanyCustomers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address2",
                table: "CompanyCustomers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "CompanyCustomers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StateCode",
                table: "CompanyCustomers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "CompanyCustomers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address1",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address2",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StateCode",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address1",
                table: "ProjectAreas");

            migrationBuilder.DropColumn(
                name: "Address2",
                table: "ProjectAreas");

            migrationBuilder.DropColumn(
                name: "City",
                table: "ProjectAreas");

            migrationBuilder.DropColumn(
                name: "StateCode",
                table: "ProjectAreas");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "ProjectAreas");

            migrationBuilder.DropColumn(
                name: "Address1",
                table: "CompanyCustomers");

            migrationBuilder.DropColumn(
                name: "Address2",
                table: "CompanyCustomers");

            migrationBuilder.DropColumn(
                name: "City",
                table: "CompanyCustomers");

            migrationBuilder.DropColumn(
                name: "StateCode",
                table: "CompanyCustomers");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "CompanyCustomers");

            migrationBuilder.DropColumn(
                name: "Address1",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Address2",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "StateCode",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "Companies");

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "CompanyCustomers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "Companies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StateCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyCustomers_AddressId",
                table: "CompanyCustomers",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_AddressId",
                table: "Companies",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Addresses_AddressId",
                table: "Companies",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyCustomers_Addresses_AddressId",
                table: "CompanyCustomers",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
