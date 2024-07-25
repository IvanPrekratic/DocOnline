using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zavrsni.DAL.Migrations
{
    /// <inheritdoc />
    public partial class BiljeskeDoktora : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BiljeskeDoktora",
                table: "Pregledi",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BiljeskeDoktora",
                table: "Pregledi");
        }
    }
}
