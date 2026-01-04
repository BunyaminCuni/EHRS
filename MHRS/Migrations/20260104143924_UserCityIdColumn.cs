using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MHRS.Migrations
{
    /// <inheritdoc />
    public partial class UserCityIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Önce eski city sütununu sil
            migrationBuilder.DropColumn(
                name: "city",
                table: "users");

            // 2. Yeni cityId sütununu ekle (başta NULL olabilir)
            migrationBuilder.AddColumn<int>(
                name: "cityId",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 34);  // DEĞİŞTİ: 0 → 34 (İstanbul)

            // 3. Tüm mevcut kullanıcıları İstanbul'a ata (emin olmak için)
            migrationBuilder.Sql("UPDATE users SET cityId = 34 WHERE cityId = 0 OR cityId IS NULL");

            // 4. Index oluştur
            migrationBuilder.CreateIndex(
                name: "IX_users_cityId",
                table: "users",
                column: "cityId");

            // 5. Foreign Key ekle
            migrationBuilder.AddForeignKey(
                name: "FK_users_cities_cityId",
                table: "users",
                column: "cityId",
                principalTable: "cities",
                principalColumn: "cityId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_cities_cityId",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_cityId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "cityId",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}