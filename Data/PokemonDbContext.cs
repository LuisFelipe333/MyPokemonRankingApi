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

            // Configuramos un índice único para la columna Position
            modelBuilder.Entity<Pokemon>()
                .HasIndex(p => p.Position)
                .IsUnique();
        }
    }


}
