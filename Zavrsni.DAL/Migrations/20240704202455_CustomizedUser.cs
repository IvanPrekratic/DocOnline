using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zavrsni.DAL.Migrations
{
    /// <inheritdoc />
    public partial class CustomizedUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DatumRodjenja",
                table: "Pacijenti",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "DoktorID",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PacijentID",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DoktorID",
                table: "AspNetUsers",
                column: "DoktorID");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PacijentID",
                table: "AspNetUsers",
                column: "PacijentID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Doktori_DoktorID",
                table: "AspNetUsers",
                column: "DoktorID",
                principalTable: "Doktori",
                principalColumn: "DoktorID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Pacijenti_PacijentID",
                table: "AspNetUsers",
                column: "PacijentID",
                principalTable: "Pacijenti",
                principalColumn: "PacijentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Doktori_DoktorID",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Pacijenti_PacijentID",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DoktorID",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PacijentID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DoktorID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PacijentID",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "DatumRodjenja",
                table: "Pacijenti",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
