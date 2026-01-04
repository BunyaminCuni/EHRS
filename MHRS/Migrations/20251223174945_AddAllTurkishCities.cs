using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MHRS.Migrations
{
    /// <inheritdoc />
    public partial class AddAllTurkishCities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "hospitalId",
                table: "appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 1,
                column: "cityName",
                value: "Adana");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 2,
                column: "cityName",
                value: "Adıyaman");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 3,
                column: "cityName",
                value: "Afyonkarahisar");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 4,
                column: "cityName",
                value: "Ağrı");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 5,
                column: "cityName",
                value: "Amasya");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 6,
                column: "cityName",
                value: "Ankara");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 7,
                column: "cityName",
                value: "Antalya");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 8,
                column: "cityName",
                value: "Artvin");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 9,
                column: "cityName",
                value: "Aydın");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 10,
                column: "cityName",
                value: "Balıkesir");

            migrationBuilder.InsertData(
                table: "cities",
                columns: new[] { "cityId", "cityName" },
                values: new object[,]
                {
                    { 11, "Bilecik" },
                    { 12, "Bingöl" },
                    { 13, "Bitlis" },
                    { 14, "Bolu" },
                    { 15, "Burdur" },
                    { 16, "Bursa" },
                    { 17, "Çanakkale" },
                    { 18, "Çankırı" },
                    { 19, "Çorum" },
                    { 20, "Denizli" },
                    { 21, "Diyarbakır" },
                    { 22, "Edirne" },
                    { 23, "Elazığ" },
                    { 24, "Erzincan" },
                    { 25, "Erzurum" },
                    { 26, "Eskişehir" },
                    { 27, "Gaziantep" },
                    { 28, "Giresun" },
                    { 29, "Gümüşhane" },
                    { 30, "Hakkari" },
                    { 31, "Hatay" },
                    { 32, "Isparta" },
                    { 33, "Mersin" },
                    { 34, "İstanbul" },
                    { 35, "İzmir" },
                    { 36, "Kars" },
                    { 37, "Kastamonu" },
                    { 38, "Kayseri" },
                    { 39, "Kırklareli" },
                    { 40, "Kırşehir" },
                    { 41, "Kocaeli" },
                    { 42, "Konya" },
                    { 43, "Kütahya" },
                    { 44, "Malatya" },
                    { 45, "Manisa" },
                    { 46, "Kahramanmaraş" },
                    { 47, "Mardin" },
                    { 48, "Muğla" },
                    { 49, "Muş" },
                    { 50, "Nevşehir" },
                    { 51, "Niğde" },
                    { 52, "Ordu" },
                    { 53, "Rize" },
                    { 54, "Sakarya" },
                    { 55, "Samsun" },
                    { 56, "Siirt" },
                    { 57, "Sinop" },
                    { 58, "Sivas" },
                    { 59, "Tekirdağ" },
                    { 60, "Tokat" },
                    { 61, "Trabzon" },
                    { 62, "Tunceli" },
                    { 63, "Şanlıurfa" },
                    { 64, "Uşak" },
                    { 65, "Van" },
                    { 66, "Yozgat" },
                    { 67, "Zonguldak" },
                    { 68, "Aksaray" },
                    { 69, "Bayburt" },
                    { 70, "Karaman" },
                    { 71, "Kırıkkale" },
                    { 72, "Batman" },
                    { 73, "Şırnak" },
                    { 74, "Bartın" },
                    { 75, "Ardahan" },
                    { 76, "Iğdır" },
                    { 77, "Yalova" },
                    { 78, "Karabük" },
                    { 79, "Kilis" },
                    { 80, "Osmaniye" },
                    { 81, "Düzce" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 81);

            migrationBuilder.DropColumn(
                name: "hospitalId",
                table: "appointments");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 1,
                column: "cityName",
                value: "İstanbul");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 2,
                column: "cityName",
                value: "Ankara");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 3,
                column: "cityName",
                value: "İzmir");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 4,
                column: "cityName",
                value: "Bursa");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 5,
                column: "cityName",
                value: "Antalya");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 6,
                column: "cityName",
                value: "Tekirdağ");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 7,
                column: "cityName",
                value: "Gaziantep");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 8,
                column: "cityName",
                value: "Konya");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 9,
                column: "cityName",
                value: "Kayseri");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "cityId",
                keyValue: 10,
                column: "cityName",
                value: "Diyarbakır");
        }
    }
}
