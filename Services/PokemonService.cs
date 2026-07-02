using System.Net.Http.Json;
using MyPokemonRankingApi.DTOs;
using MyPokemonRankingApi.Interfaces;
using MyPokemonRankingApi.Models;

namespace MyPokemonRankingApi.Services
{
    public class PokemonService : IPokemonService
    {
        private readonly IPokemonRepository _repository;
        private readonly HttpClient _httpClient;

        // Inyectamos nuestro repositorio y el HttpClient que nos dará .NET
        public PokemonService(IPokemonRepository repository, HttpClient httpClient)
        {
            _repository = repository;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Pokemon>> GetRankingAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Pokemon> AddToRankingAsync(CreatePokemonDto createDto)
        {
            // 1. TODO: Consumir la PokéAPI usando el createDto.PokemonApiId
            // 2. TODO: Aplicar la regla de negocio para recorrer las posiciones
            // 3. TODO: Guardar en la base de datos a través del repositorio
            throw new NotImplementedException();
        }

    }
}
