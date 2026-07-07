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

        public async Task<IEnumerable<Pokemon>> GetRankingAsync(int? generation = null, string? type = null)
        {
            var currentRanking = await _repository.GetAllAsync();

            if (generation.HasValue)//Filtro por generacion
            {
                currentRanking = currentRanking.Where(p => p.Generation == generation.Value);
            }

            if (!string.IsNullOrEmpty(type)) //Filtro por tipo
            {
                var typeLower = type.ToLower();

                currentRanking = currentRanking.Where(p =>
                    p.PrimaryType.ToLower() == typeLower ||
                    (p.SecondaryType != null && p.SecondaryType.ToLower() == typeLower)
                );
            }

            return currentRanking.OrderBy(p => p.Position).ToList();
        }

        public async Task<Pokemon> AddToRankingAsync(CreatePokemonDto createDto)
        {

            if (createDto.Position < 1)
            {
                throw new ArgumentException("La posición en el ranking debe ser mayor o igual a 1."); //Verificamos que envien una posicion valida
            }

            var allPokemon = await _repository.GetAllAsync();

            bool alreadyExists = allPokemon.Any(p => p.PokemonApiId == createDto.PokemonApiId); //Verificamos que el Pokémon no esté ya en el ranking

            if (alreadyExists)
            {
                throw new InvalidOperationException("Este Pokémon ya se encuentra registrado en tu ranking.");
            }

           
            int totalPlusOne = allPokemon.Count() + 1;

            if (createDto.Position > totalPlusOne) //Verificamos que la posición no sea mayor a la cantidad de Pokémon + 1
            {
                throw new ArgumentException($"Posición inválida. El ranking actual tiene {totalPlusOne} Pokémon.");
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

            // 2: Aplicar la regla de negocio para recorrer las posiciones
            
            PushDown(allPokemon, createDto.Position); //Se cambia a un metodo aparte para reusarlo

            // 3: Guardar en la base de datos a través del repositorio

            var newPokemon = new Pokemon
            {
                PokemonApiId = createDto.PokemonApiId,
                Position = createDto.Position,
                Name = char.ToUpper(pokeData.name[0]) + pokeData.name.Substring(1), // Capitalizamos la primera letra del nombre
                PrimaryType = primaryType,
                SecondaryType = secondaryType,
                Generation = GetGenerationByPokemonId(createDto.PokemonApiId) //Obtenemos la generación según el ID del Pokémon
            };

            // 4: Guardamos el nuevo Pokémon y confirmamos todos los cambios en la BD
            await _repository.AddAsync(newPokemon);
            await _repository.SaveChangesAsync();

            return newPokemon;
        }

        public async Task DeleteFromRankingAsync(int id)
        {
            var pokemonToDelete = await _repository.GetByIdAsync(id);
            if (pokemonToDelete == null)
            {
                throw new KeyNotFoundException("El Pokémon con el ID especificado no existe en el ranking.");
            }

            var allPokemon = await _repository.GetAllAsync();

            _repository.Delete(pokemonToDelete);

            PushUp(allPokemon, pokemonToDelete.Position);

            await _repository.SaveChangesAsync();
        }


        public async Task<Pokemon> UpdatePositionAsync(int id, int newPosition)
        {
            if (newPosition < 1)
            {
                throw new ArgumentException("La posición debe ser mayor o igual a 1.");
            }

            var pokemonToMove = await _repository.GetByIdAsync(id);
            if (pokemonToMove == null)
            {
                throw new KeyNotFoundException("El Pokémon con el ID especificado no existe en el ranking.");
            }

            
            var allPokemon = await _repository.GetAllAsync();
            int totalPlusOne = allPokemon.Count() + 1;

            if (newPosition > totalPlusOne)
            {
                throw new ArgumentException($"Posición inválida. El ranking actual tiene {totalPlusOne} Pokémon.");
            }

            // Si la posición es la misma retornamos
            int oldPosition = pokemonToMove.Position;
            if (oldPosition == newPosition) return pokemonToMove;

            if (newPosition < oldPosition)
            {
                // Si el pokemon sube en el ranking, movemos todos los que están entre la nueva posición y la antigua posición hacia abajo
               
                var affectedPokemon = allPokemon
                    .Where(p => p.Position >= newPosition && p.Position < oldPosition);

                PushDown(affectedPokemon, newPosition);
            }
            else
            {
                // El Pokémon BAJA en el ranking, movemos todos los que están entre la antigua posición y la nueva posición hacia arriba
                var affectedPokemon = allPokemon
                    .Where(p => p.Position > oldPosition && p.Position <= newPosition);

                
                PushUp(affectedPokemon, oldPosition);
            }

            pokemonToMove.Position = newPosition;
            _repository.Update(pokemonToMove);

            
            await _repository.SaveChangesAsync();

            return pokemonToMove;
        }



        private void PushDown(IEnumerable<Pokemon> list, int startPosition)
        {
            var affected = list
                .Where(p => p.Position >= startPosition)
                .OrderByDescending(p => p.Position); // De mayor a menor para no chocar indices unicos

            foreach (var poke in affected)
            {
                poke.Position += 1;
                _repository.Update(poke);
            }
        }

        private void PushUp(IEnumerable<Pokemon> list, int startPosition)
        {
            var affected = list
                .Where(p => p.Position > startPosition)
                .OrderBy(p => p.Position); // De menor a mayor para ir subiendo escalonadamente

            foreach (var poke in affected)
            {
                poke.Position -= 1;
                _repository.Update(poke);
            }
        }


        private int GetGenerationByPokemonId(int pokemonApiId) //Nos devuelve la generación del Pokémon según su ID en la PokéAPI
        {
            // Evaluamos el ID según los rangos oficiales de la PokéAPI
            if (pokemonApiId >= 1 && pokemonApiId <= 151) return 1;
            if (pokemonApiId >= 152 && pokemonApiId <= 251) return 2;
            if (pokemonApiId >= 252 && pokemonApiId <= 386) return 3;
            if (pokemonApiId >= 387 && pokemonApiId <= 493) return 4;
            if (pokemonApiId >= 494 && pokemonApiId <= 649) return 5;
            if (pokemonApiId >= 650 && pokemonApiId <= 721) return 6;
            if (pokemonApiId >= 722 && pokemonApiId <= 809) return 7;
            if (pokemonApiId >= 810 && pokemonApiId <= 905) return 8;
            if (pokemonApiId >= 906 && pokemonApiId <= 1025) return 9;

            // Por defecto lo dejamos en 0 o 9
            return 9;
        }




    }

    // Clases para mapear la respuesta de la PokéAPI
    public record PokeApiPokemonResponse(string name, List<PokeApiTypeSlot> types);
    public record PokeApiTypeSlot(int slot, PokeApiType type);
    public record PokeApiType(string name);


}
