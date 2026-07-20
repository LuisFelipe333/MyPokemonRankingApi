using MyPokemonRankingApi.Models;

namespace MyPokemonRankingApi.Interfaces
{
    public interface IPokemonRepository
    {
        Task<IEnumerable<Pokemon>> GetAllAsync();
        Task<Pokemon?> GetByIdAsync(int id);
        Task<Pokemon?> GetByPositionAsync(int position);
        Task AddAsync(Pokemon pokemon);
        void Update(Pokemon pokemon);
        void Delete(Pokemon pokemon);
        Task<bool> SaveChangesAsync();
        Task<IEnumerable<Pokemon>> GetByUserIdAsync(string userId); // Obtiene todos los Pokémon de un usuario específico

        Task<Pokemon?> GetByIdAndUserIdAsync(int id, string userId); // Obtiene un Pokémon específico por su ID y el ID del usuario

    }
}
