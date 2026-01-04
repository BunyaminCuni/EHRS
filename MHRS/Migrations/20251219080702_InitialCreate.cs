using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MHRS.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cities",
                columns: table => new
                {
                    cityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cityName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cities", x => x.cityId);
                });

            migrationBuilder.CreateTable(
                name: "patient",
                columns: table => new
                {
                    patientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    patientName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    mobileNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    city = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    address = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patient", x => x.patientId);
                });

            migrationBuilder.CreateTable(
                name: "hospitals",
                columns: table => new
                {
                    hospitalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    hospitalName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    cityId = table.Column<int>(type: "int", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hospitals", x => x.hospitalId);
                    table.ForeignKey(
                        name: "FK_hospitals_cities_cityId",
                        column: x => x.cityId,
                        principalTable: "cities",
                        principalColumn: "cityId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "appointments",
                columns: table => new
                {
                    appointmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    patientId = table.Column<int>(type: "int", nullable: false),
                    appointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isDone = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointments", x => x.appointmentId);
                    table.ForeignKey(
                        name: "FK_appointments_patient_patientId",
                        column: x => x.patientId,
                        principalTable: "patient",
                        principalColumn: "patientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "cities",
                columns: new[] { "cityId", "cityName" },
                values: new object[,]
                {
                    { 1, "İstanbul" },
                    { 2, "Ankara" },
                    { 3, "İzmir" },
                    { 4, "Bursa" },
                    { 5, "Antalya" },
                    { 6, "Tekirdağ" },
                    { 7, "Gaziantep" },
                    { 8, "Konya" },
                    { 9, "Kayseri" },
                    { 10, "Diyarbakır" }
                });

            migrationBuilder.InsertData(
                table: "hospitals",
                columns: new[] { "hospitalId", "address", "cityId", "description", "hospitalName", "phone" },
                values: new object[,]
                {
                    { 1, "İstanbul, Kadıköy", 1, "Modern veteriner hastanesi", "Acibadem Veteriner Hastanesi", "0212-555-0001" },
                    { 2, "İstanbul, Nişantaşı", 1, "Uluslararası standartlarda hizmet", "American Hospital Vet", "0212-555-0002" },
                    { 3, "Ankara, Keçiören", 2, "Ankara'nın en iyi veteriner merkezi", "Ankara Veteriner Merkezi", "0312-555-0001" },
                    { 4, "İzmir, Alsancak", 3, "Evcil hayvanlar için özel hizmetler", "İzmir Pet Hospital", "0232-555-0001" },
                    { 5, "Tekirdağ, Merkez", 6, "Tekirdağ'da güvenilir veteriner hizmeti", "Tekirdağ Vet Kliniği", "0282-555-0001" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_appointments_patientId",
                table: "appointments",
                column: "patientId");

            migrationBuilder.CreateIndex(
                name: "IX_hospitals_cityId",
                table: "hospitals",
                column: "cityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointments");

            migrationBuilder.DropTable(
                name: "hospitals");

            migrationBuilder.DropTable(
                name: "patient");

            migrationBuilder.DropTable(
                name: "cities");
        }
    }
}
