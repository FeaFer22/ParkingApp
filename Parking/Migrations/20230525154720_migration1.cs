using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParkingApp.Migrations
{
    /// <inheritdoc />
    public partial class migration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FixedSlots_ParkingSlots_ParkingSlotId",
                table: "FixedSlots");

            migrationBuilder.DropIndex(
                name: "IX_FixedSlots_ParkingSlotId",
                table: "FixedSlots");

            migrationBuilder.DropColumn(
                name: "ParkingSlotId",
                table: "FixedSlots");

            migrationBuilder.AddColumn<string>(
                name: "ParkingSlotName",
                table: "FixedSlots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParkingSlotName",
                table: "FixedSlots");

            migrationBuilder.AddColumn<int>(
                name: "ParkingSlotId",
                table: "FixedSlots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FixedSlots_ParkingSlotId",
                table: "FixedSlots",
                column: "ParkingSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_FixedSlots_ParkingSlots_ParkingSlotId",
                table: "FixedSlots",
                column: "ParkingSlotId",
                principalTable: "ParkingSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
