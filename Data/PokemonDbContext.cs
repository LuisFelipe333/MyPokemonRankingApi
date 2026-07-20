using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyPokemonRankingApi.Models;

namespace MyPokemonRankingApi.Data
{
    public class PokemonDbContext : IdentityDbContext
    {
        public PokemonDbContext(DbContextOptions<PokemonDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pokemon> Pokemons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Pokemon>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade); //Se relaciona los Pokémon con el usuario y se establece la eliminación en cascada para que al eliminar un usuario, también se eliminen sus Pokémon asociados.

            //Se quita el indice único en la propiedad Position para permitir que se puedan tener posiciones duplicadas temporalmente durante las operaciones de actualización de ranking.
        }
    }


}
