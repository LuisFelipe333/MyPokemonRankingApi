using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPokemonRankingApi.Migrations
{
    /// <inheritdoc />
    public partial class AddGenerationToPokemon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Generation",
                table: "Pokemons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Pokemons_Position",
                table: "Pokemons",
                column: "Position",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pokemons_Position",
                table: "Pokemons");

            migrationBuilder.DropColumn(
                name: "Generation",
                table: "Pokemons");
        }
    }
}
