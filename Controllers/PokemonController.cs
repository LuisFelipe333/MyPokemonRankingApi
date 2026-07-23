using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPokemonRankingApi.DTOs;
using MyPokemonRankingApi.Interfaces;
using System.Security.Claims;

namespace MyPokemonRankingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Solicita autenticación para todas las acciones del controlador, exige JWT válido.
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonService _pokemonService;

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!; // Obtiene el ID del usuario autenticado desde el token JWT.

        public PokemonController(IPokemonService pokemonService)
        {
            _pokemonService = pokemonService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRanking([FromQuery] int? generation = null, [FromQuery] string? type = null)
        {
            try
            {
                var userId = GetUserId();
                var ranking = await _pokemonService.GetRankingAsync(userId, generation, type);
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
                var userId = GetUserId();
                var nuevoPokemon = await _pokemonService.AddToRankingAsync(userId, createDto);
                return CreatedAtAction(nameof(GetRanking), null, nuevoPokemon);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFromRanking(int id)
        {
            try
            {
                var userId = GetUserId();
                await _pokemonService.DeleteFromRankingAsync(userId, id);
                return NoContent(); 
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // HTTP 404
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message }); // HTTP 400
            }
        }

        [HttpPut("{id}/position")]
        public async Task<IActionResult> UpdatePosition(int id, [FromBody] int newPosition)
        {
            try
            {
                var userId = GetUserId();
                var updatedPokemon = await _pokemonService.UpdatePositionAsync(userId, id, newPosition);
                return Ok(updatedPokemon);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal error: {ex.Message}" });
            }
        }

        [HttpGet("share/{username}")]
        [AllowAnonymous] // Permite el acceso sin token JWT
        public async Task<IActionResult> GetPublicRanking(string username)
        {
            try
            {
                var ranking = await _pokemonService.GetPublicRankingAsync(username);
                return Ok(ranking);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

    }
}
