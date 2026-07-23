using MyPokemonRankingApi.DTOs;
using MyPokemonRankingApi.Models;

namespace MyPokemonRankingApi.Interfaces
{
    public interface IPokemonService
    {
        Task<PokemonDto> AddToRankingAsync(string userId, CreatePokemonDto createDto);
        Task<IEnumerable<PokemonDto>> GetPublicRankingAsync(string username);
        Task<IEnumerable<PokemonDto>> GetRankingAsync(string userId, int? generation = null, string? type = null);
        Task DeleteFromRankingAsync(string userId, int id);
        Task<PokemonDto> UpdatePositionAsync(string userId, int id, int newPosition);
    }
}
