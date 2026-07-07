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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Se quita el indice único en la propiedad Position para permitir que se puedan tener posiciones duplicadas temporalmente durante las operaciones de actualización de ranking.
        }
    }


}
