using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zavrsni.DAL.Migrations
{
    /// <inheritdoc />
    public partial class PregledPotvrdeno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Potvrdeno",
                table: "Pregledi",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Potvrdeno",
                table: "Pregledi");
        }
    }
}
