using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPokemonRankingApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueIndexFromPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pokemons_Position",
                table: "Pokemons");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Pokemons_Position",
                table: "Pokemons",
                column: "Position",
                unique: true);
        }
    }
}
