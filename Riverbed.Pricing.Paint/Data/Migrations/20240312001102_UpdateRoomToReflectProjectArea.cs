using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riverbed.Pricing.Paint.Shared.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoomToReflectProjectArea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_ProjectAreas_ProjectAreaId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_ProjectAreaId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "ProjectAreaId",
                table: "Rooms",
                newName: "ProjectDataId");

            migrationBuilder.RenameColumn(
                name: "PaintQuality",
                table: "Rooms",
                newName: "PictureId");

            migrationBuilder.AlterColumn<int>(
                name: "Width",
                table: "Rooms",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "Length",
                table: "Rooms",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "Height",
                table: "Rooms",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<float>(
                name: "AreaTotal",
                table: "Rooms",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsChangeOrder",
                table: "Rooms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOptional",
                table: "Rooms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<float>(
                name: "LaborCost",
                table: "Rooms",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "MaterialCost",
                table: "Rooms",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "PrimeWalls",
                table: "Rooms",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AreaTotal",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "IsChangeOrder",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "IsOptional",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "LaborCost",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "MaterialCost",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "PrimeWalls",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "ProjectDataId",
                table: "Rooms",
                newName: "ProjectAreaId");

            migrationBuilder.RenameColumn(
                name: "PictureId",
                table: "Rooms",
                newName: "PaintQuality");

            migrationBuilder.AlterColumn<double>(
                name: "Width",
                table: "Rooms",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "Length",
                table: "Rooms",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "Height",
                table: "Rooms",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Rooms",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_ProjectAreaId",
                table: "Rooms",
                column: "ProjectAreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_ProjectAreas_ProjectAreaId",
                table: "Rooms",
                column: "ProjectAreaId",
                principalTable: "ProjectAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
