using MyPokemonRankingApi.DTOs;
using MyPokemonRankingApi.Models;

namespace MyPokemonRankingApi.Interfaces
{
    public interface IPokemonService
    {
        Task<Pokemon> AddToRankingAsync(string userId, CreatePokemonDto createDto);
        Task<IEnumerable<Pokemon>> GetRankingAsync(string userId, int? generation = null, string? type = null);
        Task DeleteFromRankingAsync(string userId, int id);
        Task<Pokemon> UpdatePositionAsync(string userId, int id, int newPosition);
    }
}
