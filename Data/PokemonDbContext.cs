using Microsoft.EntityFrameworkCore;
using MyPokemonRankingApi.Models;

namespace MyPokemonRankingApi.Data
{
    public class PokemonDbContext : DbContext
    {
        public PokemonDbContext(DbContextOptions<PokemonDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pokemon> Pokemons { get; set; }
    }
}
