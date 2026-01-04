using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MHRS.Migrations
{
    /// <inheritdoc />
    public partial class FixHospitalModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "hospitals",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "hospitalName",
                table: "hospitals",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "hospitals",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.UpdateData(
                table: "hospitals",
                keyColumn: "hospitalId",
                keyValue: 1,
                column: "cityId",
                value: 34);

            migrationBuilder.UpdateData(
                table: "hospitals",
                keyColumn: "hospitalId",
                keyValue: 2,
                column: "cityId",
                value: 34);

            migrationBuilder.UpdateData(
                table: "hospitals",
                keyColumn: "hospitalId",
                keyValue: 3,
                column: "cityId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "hospitals",
                keyColumn: "hospitalId",
                keyValue: 4,
                column: "cityId",
                value: 35);

            migrationBuilder.UpdateData(
                table: "hospitals",
                keyColumn: "hospitalId",
                keyValue: 5,
                column: "cityId",
                value: 59);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "hospitals",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "hospitalName",
                table: "hospitals",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "hospitals",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.UpdateData(
                table: "hospitals",
                keyColumn: "hospitalId",
                keyValue: 1,
                column: "cityId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "hospitals",
                keyColumn: "hospitalId",
                keyValue: 2,
                column: "cityId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "hospitals",
                keyColumn: "hospitalId",
                keyValue: 3,
                column: "cityId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "hospitals",
                keyColumn: "hospitalId",
                keyValue: 4,
                column: "cityId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "hospitals",
                keyColumn: "hospitalId",
                keyValue: 5,
                column: "cityId",
                value: 6);
        }
    }
}
