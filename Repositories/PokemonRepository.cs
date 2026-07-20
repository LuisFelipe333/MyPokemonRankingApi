using MyPokemonRankingApi.Data;
using MyPokemonRankingApi.Interfaces;
using MyPokemonRankingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MyPokemonRankingApi.Repositories
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly PokemonDbContext _context;

        // Le pedimos el DbContext al sistema
        public PokemonRepository(PokemonDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Pokemon>> GetByUserIdAsync(string userId)
        {
            return await _context.Pokemons
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<Pokemon?> GetByIdAndUserIdAsync(int id, string userId)
        {
            return await _context.Pokemons
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        }

        public async Task<IEnumerable<Pokemon>> GetAllAsync()
        {
            return await _context.Pokemons.OrderBy(p => p.Position).ToListAsync();
        }

        public async Task<Pokemon?> GetByIdAsync(int id)
        {
            return await _context.Pokemons.FindAsync(id);
        }

        public async Task<Pokemon?> GetByPositionAsync(int position)
        {
            return await _context.Pokemons.FirstOrDefaultAsync(p => p.Position == position);
        }

        public async Task AddAsync(Pokemon pokemon)
        {
            await _context.Pokemons.AddAsync(pokemon);
        }

        public void Update(Pokemon pokemon)
        {
            _context.Pokemons.Update(pokemon);
        }

        public void Delete(Pokemon pokemon)
        {
            _context.Pokemons.Remove(pokemon);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
