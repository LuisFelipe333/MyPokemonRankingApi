using System.ComponentModel.DataAnnotations;

namespace MyPokemonRankingApi.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "The username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "The email format is invalid.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The password is required.")]
        [MinLength(6, ErrorMessage = "The password must be at least 6 characters.")]
        public string Password { get; set; }
    }
}
