using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MHRS.Migrations
{
    /// <inheritdoc />
    public partial class RenamePatientToUserSafe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ============================================
            // SADECE PATIENT → USERS DÖNÜŞTÜRMESİ
            // ============================================

            // 1. patient tablosunu users olarak yeniden adlandır
            migrationBuilder.RenameTable(
                name: "patient",
                newName: "users");

            // 2. Kolonları yeniden adlandır
            migrationBuilder.RenameColumn(
                name: "patientId",
                table: "users",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "patientName",
                table: "users",
                newName: "userName");

            migrationBuilder.RenameColumn(
                name: "mobileNo",
                table: "users",
                newName: "phone");

            // 3. Yeni kolonlar ekle
            migrationBuilder.AddColumn<string>(
                name: "passwordHash",
                table: "users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "createdAt",
                table: "users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2025, 1, 1));

            // 4. email kolonunu genişlet
            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            // 5. phone için unique index ekle
            migrationBuilder.CreateIndex(
                name: "IX_users_phone",
                table: "users",
                column: "phone",
                unique: true);

            // ============================================
            // SADECE PETS.OWNERPHONE → PETS.USERID
            // ============================================

            // 6. pets.ownerPhone kolonunu sil
            migrationBuilder.DropColumn(
                name: "ownerPhone",
                table: "pets");

            // 7. pets.userId kolonunu ekle
            migrationBuilder.AddColumn<int>(
                name: "userId",
                table: "pets",
                type: "int",
                nullable: false,
                defaultValue: 1);

            // 8. pets.userId için foreign key ve index ekle
            migrationBuilder.CreateIndex(
                name: "IX_pets_userId",
                table: "pets",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_pets_users_userId",
                table: "pets",
                column: "userId",
                principalTable: "users",
                principalColumn: "userId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rollback işlemleri

            // pets FK'yi kaldır
            migrationBuilder.DropForeignKey(
                name: "FK_pets_users_userId",
                table: "pets");

            // pets düzenlemeleri geri al
            migrationBuilder.DropIndex(
                name: "IX_pets_userId",
                table: "pets");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "pets");

            migrationBuilder.AddColumn<string>(
                name: "ownerPhone",
                table: "pets",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            // users düzenlemeleri geri al
            migrationBuilder.DropIndex(
                name: "IX_users_phone",
                table: "users");

            migrationBuilder.DropColumn(
                name: "passwordHash",
                table: "users");

            migrationBuilder.DropColumn(
                name: "createdAt",
                table: "users");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "users",
                newName: "patientId");

            migrationBuilder.RenameColumn(
                name: "userName",
                table: "users",
                newName: "patientName");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "users",
                newName: "mobileNo");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "patient");
        }
    }
}