using MyPokemonRankingApi.DTOs;
using MyPokemonRankingApi.Models;

namespace MyPokemonRankingApi.Interfaces
{
    public interface IPokemonService
    {
        Task<Pokemon> AddToRankingAsync(CreatePokemonDto createDto);
        Task<IEnumerable<Pokemon>> GetRankingAsync(int? generation = null, string? type = null);
        Task DeleteFromRankingAsync(int id);
        Task<Pokemon> UpdatePositionAsync(int id, int newPosition);
    }
}
