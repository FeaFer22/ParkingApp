using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParkingApp.Migrations
{
    /// <inheritdoc />
    public partial class migration0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FixedSlots_Users_UserId",
                table: "FixedSlots");

            migrationBuilder.DropIndex(
                name: "IX_FixedSlots_UserId",
                table: "FixedSlots");

            migrationBuilder.DropColumn(
                name: "IsFixed",
                table: "FixedSlots");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FixedSlots");

            migrationBuilder.AddColumn<bool>(
                name: "IsFixed",
                table: "ParkingSlots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FixationEndTime",
                table: "FixedSlots",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserLicensePlate",
                table: "FixedSlots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFixed",
                table: "ParkingSlots");

            migrationBuilder.DropColumn(
                name: "FixationEndTime",
                table: "FixedSlots");

            migrationBuilder.DropColumn(
                name: "UserLicensePlate",
                table: "FixedSlots");

            migrationBuilder.AddColumn<bool>(
                name: "IsFixed",
                table: "FixedSlots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "FixedSlots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FixedSlots_UserId",
                table: "FixedSlots",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FixedSlots_Users_UserId",
                table: "FixedSlots",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
