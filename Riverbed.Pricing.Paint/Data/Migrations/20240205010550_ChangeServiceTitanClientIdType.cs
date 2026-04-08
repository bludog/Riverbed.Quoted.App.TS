using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class ChangeServiceTitanClientIdType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceTitanApiCompanyId",
                table: "ServiceTitanConnectionDatas");

            migrationBuilder.AddColumn<int>(
                name: "ServiceTitanApiClientId",
                table: "ServiceTitanConnectionDatas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceTitanApiClientId",
                table: "ServiceTitanConnectionDatas");

            migrationBuilder.AddColumn<string>(
                name: "ServiceTitanApiCompanyId",
                table: "ServiceTitanConnectionDatas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
