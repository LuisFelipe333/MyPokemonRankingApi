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
            // Consumir la PokéAPI usando el createDto.PokemonApiId
            var url = $"https://pokeapi.co/api/v2/pokemon/{createDto.PokemonApiId}";

            var pokeData = await _httpClient.GetFromJsonAsync<PokeApiPokemonResponse>(url);

            if (pokeData == null)
            {
                throw new Exception("No se encontró el Pokémon en la PokéAPI externa.");
            }

            var tipos = pokeData.types
                .OrderBy(t => t.slot)
                .Select(t => t.type.name)
                .ToList();

            string primaryType = tipos.FirstOrDefault() ?? "Unknown";
            string? secondaryType = tipos.Count > 1 ? tipos[1] : null;

            // 2. TODO: Aplicar la regla de negocio para recorrer las posiciones
            
            var pokemonAfectados = await _repository.GetAllAsync(); //Traer todos los pokemones para poder recorrerlos

            var paraRecorrer = pokemonAfectados
                .Where(p => p.Position >= createDto.Position)
                .OrderByDescending(p => p.Position); //Ordenamos de mayor a menor para poder recorrerlos y sumarle 1 a su posición

            foreach (var poke in paraRecorrer) //Recorremos los pokemones que tienen una posición mayor o igual a la que queremos insertar
            {
                poke.Position += 1;
                _repository.Update(poke);
            }

            // 3. TODO: Guardar en la base de datos a través del repositorio

            var nuevoPokemon = new Pokemon
            {
                PokemonApiId = createDto.PokemonApiId,
                Position = createDto.Position,
                Name = char.ToUpper(pokeData.name[0]) + pokeData.name.Substring(1), // Capitalizamos la primera letra del nombre
                PrimaryType = primaryType,
                SecondaryType = secondaryType
            };

            // 4. Guardamos el nuevo Pokémon y confirmamos todos los cambios en la BD
            await _repository.AddAsync(nuevoPokemon);
            await _repository.SaveChangesAsync();

            return nuevoPokemon;
        }

    }

    // Clases para mapear la respuesta de la PokéAPI
    public record PokeApiPokemonResponse(string name, List<PokeApiTypeSlot> types);
    public record PokeApiTypeSlot(int slot, PokeApiType type);
    public record PokeApiType(string name);


}
