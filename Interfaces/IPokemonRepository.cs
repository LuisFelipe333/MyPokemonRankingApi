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

    }
}
