using System.ComponentModel.DataAnnotations;

namespace MyPokemonRankingApi.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "The username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "The password is required.")]
        public string Password { get; set; }
    }
}