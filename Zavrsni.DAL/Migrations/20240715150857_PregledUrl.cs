using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zavrsni.DAL.Migrations
{
    /// <inheritdoc />
    public partial class PregledUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlVideopoziva",
                table: "Pregledi",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlVideopoziva",
                table: "Pregledi");
        }
    }
}
