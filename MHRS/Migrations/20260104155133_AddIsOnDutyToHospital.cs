using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MHRS.Migrations
{
    /// <inheritdoc />
    public partial class AddIsOnDutyToHospital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isOnDuty",
                table: "hospitals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "hospitals",
                keyColumn: "hospitalId",
                keyValue: 1,
                column: "isOnDuty",
                value: false);

            migrationBuilder.UpdateData(
                table: "hospitals",
                keyColumn: "hospitalId",
                keyValue: 2,
                column: "isOnDuty",
                value: false);

            migrationBuilder.UpdateData(
                table: "hospitals",
                keyColumn: "hospitalId",
                keyValue: 3,
                column: "isOnDuty",
                value: false);

            migrationBuilder.UpdateData(
                table: "hospitals",
                keyColumn: "hospitalId",
                keyValue: 4,
                column: "isOnDuty",
                value: false);

            migrationBuilder.UpdateData(
                table: "hospitals",
                keyColumn: "hospitalId",
                keyValue: 5,
                column: "isOnDuty",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isOnDuty",
                table: "hospitals");
        }
    }
}
