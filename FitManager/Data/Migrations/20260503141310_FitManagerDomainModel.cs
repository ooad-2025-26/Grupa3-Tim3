using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class FitManagerDomainModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Korisnik",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdClanarine = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    KorisnickoIme = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Uloga = table.Column<int>(type: "int", nullable: false),
                    Ime = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Prezime = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DatumRodjenja = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DatumRegistracije = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Trener_Ime = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Trener_Prezime = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnik", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipClanarine",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<int>(type: "int", nullable: false),
                    TrajanjeDana = table.Column<int>(type: "int", nullable: false),
                    Cijena = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipClanarine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dolazak",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VrijemeDolaska = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dolazak", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dolazak_Korisnik_ClanId",
                        column: x => x.ClanId,
                        principalTable: "Korisnik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GrupniTrening",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    MaksKapacitet = table.Column<int>(type: "int", nullable: false),
                    SlobodnaMjesta = table.Column<int>(type: "int", nullable: false),
                    DatumVrijeme = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Trajanje = table.Column<int>(type: "int", nullable: false),
                    TipTreninga = table.Column<int>(type: "int", nullable: false),
                    TrenerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupniTrening", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrupniTrening_Korisnik_TrenerId",
                        column: x => x.TrenerId,
                        principalTable: "Korisnik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Izvjestaj",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipIzvjestaja = table.Column<int>(type: "int", nullable: false),
                    DatumOd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatumDo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatumGenerisan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sadrzaj = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    AdministratorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Izvjestaj", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Izvjestaj_Korisnik_AdministratorId",
                        column: x => x.AdministratorId,
                        principalTable: "Korisnik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlanTreninga",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FitnessCilj = table.Column<int>(type: "int", nullable: false),
                    Bmi = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BmiKategorija = table.Column<int>(type: "int", nullable: false),
                    Intenzitet = table.Column<int>(type: "int", nullable: false),
                    SedmicniPlan = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanTreninga", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanTreninga_Korisnik_ClanId",
                        column: x => x.ClanId,
                        principalTable: "Korisnik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QRKod",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kod = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DatumGenerisanja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aktivan = table.Column<bool>(type: "bit", nullable: false),
                    ClanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QRKod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QRKod_Korisnik_ClanId",
                        column: x => x.ClanId,
                        principalTable: "Korisnik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Clanarina",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatumPocetka = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatumIsteka = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cijena = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ObavjestenjePoslano = table.Column<bool>(type: "bit", nullable: false),
                    ClanId = table.Column<int>(type: "int", nullable: false),
                    TipClanarineId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clanarina", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clanarina_Korisnik_ClanId",
                        column: x => x.ClanId,
                        principalTable: "Korisnik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clanarina_TipClanarine_TipClanarineId",
                        column: x => x.TipClanarineId,
                        principalTable: "TipClanarine",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rezervacija",
                columns: table => new
                {
                    ClanId = table.Column<int>(type: "int", nullable: false),
                    GrupniTreningId = table.Column<int>(type: "int", nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezervacija", x => new { x.ClanId, x.GrupniTreningId });
                    table.ForeignKey(
                        name: "FK_Rezervacija_GrupniTrening_GrupniTreningId",
                        column: x => x.GrupniTreningId,
                        principalTable: "GrupniTrening",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rezervacija_Korisnik_ClanId",
                        column: x => x.ClanId,
                        principalTable: "Korisnik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmailObavjestenje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatumSlanja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Sadrzaj = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    PokusajSlanja = table.Column<int>(type: "int", nullable: false),
                    ClanarinaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailObavjestenje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailObavjestenje_Clanarina_ClanarinaId",
                        column: x => x.ClanarinaId,
                        principalTable: "Clanarina",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clanarina_ClanId",
                table: "Clanarina",
                column: "ClanId");

            migrationBuilder.CreateIndex(
                name: "IX_Clanarina_TipClanarineId",
                table: "Clanarina",
                column: "TipClanarineId");

            migrationBuilder.CreateIndex(
                name: "IX_Dolazak_ClanId",
                table: "Dolazak",
                column: "ClanId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailObavjestenje_ClanarinaId",
                table: "EmailObavjestenje",
                column: "ClanarinaId");

            migrationBuilder.CreateIndex(
                name: "IX_GrupniTrening_TrenerId",
                table: "GrupniTrening",
                column: "TrenerId");

            migrationBuilder.CreateIndex(
                name: "IX_Izvjestaj_AdministratorId",
                table: "Izvjestaj",
                column: "AdministratorId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanTreninga_ClanId",
                table: "PlanTreninga",
                column: "ClanId");

            migrationBuilder.CreateIndex(
                name: "IX_QRKod_ClanId",
                table: "QRKod",
                column: "ClanId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QRKod_Kod",
                table: "QRKod",
                column: "Kod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacija_GrupniTreningId",
                table: "Rezervacija",
                column: "GrupniTreningId");

            migrationBuilder.CreateIndex(
                name: "IX_TipClanarine_Naziv",
                table: "TipClanarine",
                column: "Naziv",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dolazak");

            migrationBuilder.DropTable(
                name: "EmailObavjestenje");

            migrationBuilder.DropTable(
                name: "Izvjestaj");

            migrationBuilder.DropTable(
                name: "PlanTreninga");

            migrationBuilder.DropTable(
                name: "QRKod");

            migrationBuilder.DropTable(
                name: "Rezervacija");

            migrationBuilder.DropTable(
                name: "Clanarina");

            migrationBuilder.DropTable(
                name: "GrupniTrening");

            migrationBuilder.DropTable(
                name: "TipClanarine");

            migrationBuilder.DropTable(
                name: "Korisnik");
        }
    }
}
