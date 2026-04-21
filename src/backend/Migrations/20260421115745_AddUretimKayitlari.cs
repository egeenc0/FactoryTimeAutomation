using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fabrika.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUretimKayitlari : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UretimKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalisanId = table.Column<int>(type: "int", nullable: false),
                    VeriTipi = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    MakineTipi = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    MakineAdi = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    DirencDegeri = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    PlaceholderMetin = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SaatDilimi = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    OlusturulmaUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UretimKayitlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Urunler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Fiyat = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urunler", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UretimKayitlari");

            migrationBuilder.DropTable(
                name: "Urunler");
        }
    }
}
