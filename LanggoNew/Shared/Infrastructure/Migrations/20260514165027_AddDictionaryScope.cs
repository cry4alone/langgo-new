using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LanggoNew.Migrations
{
    /// <inheritdoc />
    public partial class AddDictionaryScope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Scope",
                table: "Dictionaries",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Scope",
                table: "Dictionaries");
        }
    }
}
