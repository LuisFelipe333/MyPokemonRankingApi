using Microsoft.AspNetCore.Mvc;
using MyPokemonRankingApi.Interfaces;
using MyPokemonRankingApi.DTOs;

namespace MyPokemonRankingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonService _pokemonService;

        public PokemonController(IPokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRanking([FromQuery] int? generation = null, [FromQuery] string? type = null)
        {
            try
            {
                var ranking = await _pokemonService.GetRankingAsync(generation, type);
                return Ok(ranking);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToRanking([FromBody] CreatePokemonDto createDto)
        {
            try
            {
                var nuevoPokemon = await _pokemonService.AddToRankingAsync(createDto);
                return CreatedAtAction(nameof(GetRanking), null, nuevoPokemon);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
