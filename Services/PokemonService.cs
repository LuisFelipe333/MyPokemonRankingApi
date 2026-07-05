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

            if (createDto.Position < 1)
            {
                throw new ArgumentException("La posición en el ranking debe ser mayor o igual a 1."); //Verificamos que envien una posicion valida
            }

            var AllPokemon = await _repository.GetAllAsync();

            bool alreadyExists = AllPokemon.Any(p => p.PokemonApiId == createDto.PokemonApiId); //Verificamos que el Pokémon no esté ya en el ranking

            if (alreadyExists)
            {
                throw new InvalidOperationException("Este Pokémon ya se encuentra registrado en tu ranking.");
            }


            // Consumir la PokéAPI usando el createDto.PokemonApiId
            var url = $"https://pokeapi.co/api/v2/pokemon/{createDto.PokemonApiId}";

            var pokeData = await _httpClient.GetFromJsonAsync<PokeApiPokemonResponse>(url);

            if (pokeData == null)
            {
                throw new Exception("No se encontró el Pokémon en la PokéAPI externa.");
            }

            var types = pokeData.types
                .OrderBy(t => t.slot)
                .Select(t => t.type.name)
                .ToList();

            string primaryType = types.FirstOrDefault() ?? "Unknown";
            string? secondaryType = types.Count > 1 ? types[1] : null;

            // 2. TODO: Aplicar la regla de negocio para recorrer las posiciones
            
            var affectedPokemon = await _repository.GetAllAsync(); //Traer todos los pokemones para poder recorrerlos

            var forMove = affectedPokemon
                .Where(p => p.Position >= createDto.Position)
                .OrderByDescending(p => p.Position); //Ordenamos de mayor a menor para poder recorrerlos y sumarle 1 a su posición

            foreach (var poke in forMove) //Recorremos los pokemones que tienen una posición mayor o igual a la que queremos insertar
            {
                poke.Position += 1;
                _repository.Update(poke);
            }

            // 3. TODO: Guardar en la base de datos a través del repositorio

            var newPokemon = new Pokemon
            {
                PokemonApiId = createDto.PokemonApiId,
                Position = createDto.Position,
                Name = char.ToUpper(pokeData.name[0]) + pokeData.name.Substring(1), // Capitalizamos la primera letra del nombre
                PrimaryType = primaryType,
                SecondaryType = secondaryType
            };

            // 4. Guardamos el nuevo Pokémon y confirmamos todos los cambios en la BD
            await _repository.AddAsync(newPokemon);
            await _repository.SaveChangesAsync();

            return newPokemon;
        }

    }

    // Clases para mapear la respuesta de la PokéAPI
    public record PokeApiPokemonResponse(string name, List<PokeApiTypeSlot> types);
    public record PokeApiTypeSlot(int slot, PokeApiType type);
    public record PokeApiType(string name);


}
