using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Zavrsni.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pacijenti",
                columns: table => new
                {
                    PacijentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Grad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Drzava = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JMBG = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KorisnickoIme = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumRodjenja = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pacijenti", x => x.PacijentID);
                });

            migrationBuilder.CreateTable(
                name: "Specijalizacije",
                columns: table => new
                {
                    SpecijalizacijaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specijalizacije", x => x.SpecijalizacijaID);
                });

            migrationBuilder.CreateTable(
                name: "Doktori",
                columns: table => new
                {
                    DoktorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecijalizacijaID = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Grad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Drzava = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JMBG = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KorisnickoIme = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Titula = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PocetakRadnogVremena = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KrajRadnogVremena = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doktori", x => x.DoktorID);
                    table.ForeignKey(
                        name: "FK_Doktori_Specijalizacije_SpecijalizacijaID",
                        column: x => x.SpecijalizacijaID,
                        principalTable: "Specijalizacije",
                        principalColumn: "SpecijalizacijaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoktorPacijent",
                columns: table => new
                {
                    DoktorID = table.Column<int>(type: "int", nullable: false),
                    PacijentID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoktorPacijent", x => new { x.DoktorID, x.PacijentID });
                    table.ForeignKey(
                        name: "FK_DoktorPacijent_Doktori_DoktorID",
                        column: x => x.DoktorID,
                        principalTable: "Doktori",
                        principalColumn: "DoktorID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DoktorPacijent_Pacijenti_PacijentID",
                        column: x => x.PacijentID,
                        principalTable: "Pacijenti",
                        principalColumn: "PacijentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pregledi",
                columns: table => new
                {
                    PregledID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatumIVrijemePregleda = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Napomena = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PacijentID = table.Column<int>(type: "int", nullable: false),
                    DoktorID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pregledi", x => x.PregledID);
                    table.ForeignKey(
                        name: "FK_Pregledi_Doktori_DoktorID",
                        column: x => x.DoktorID,
                        principalTable: "Doktori",
                        principalColumn: "DoktorID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pregledi_Pacijenti_PacijentID",
                        column: x => x.PacijentID,
                        principalTable: "Pacijenti",
                        principalColumn: "PacijentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Specijalizacije",
                columns: new[] { "SpecijalizacijaID", "Naziv" },
                values: new object[,]
                {
                    { 1, "Opća praksa" },
                    { 2, "Kardiologija" },
                    { 3, "Dermatologija" },
                    { 4, "Ginekologija" },
                    { 5, "Interna medicina" },
                    { 6, "Neurologija" },
                    { 7, "Oftalmologija" },
                    { 8, "Ortopedija" },
                    { 9, "Otorinolaringologija" },
                    { 10, "Pedijatrija" },
                    { 11, "Psihijatrija" },
                    { 12, "Radiologija" },
                    { 13, "Stomatologija" },
                    { 14, "Urologija" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Doktori_SpecijalizacijaID",
                table: "Doktori",
                column: "SpecijalizacijaID");

            migrationBuilder.CreateIndex(
                name: "IX_DoktorPacijent_PacijentID",
                table: "DoktorPacijent",
                column: "PacijentID");

            migrationBuilder.CreateIndex(
                name: "IX_Pregledi_DoktorID",
                table: "Pregledi",
                column: "DoktorID");

            migrationBuilder.CreateIndex(
                name: "IX_Pregledi_PacijentID",
                table: "Pregledi",
                column: "PacijentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoktorPacijent");

            migrationBuilder.DropTable(
                name: "Pregledi");

            migrationBuilder.DropTable(
                name: "Doktori");

            migrationBuilder.DropTable(
                name: "Pacijenti");

            migrationBuilder.DropTable(
                name: "Specijalizacije");
        }
    }
}
