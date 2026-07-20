using Microsoft.AspNetCore.Identity;

namespace MyPokemonRankingApi.Models
{
    public class Pokemon
    {
        public int Id { get; set; }

        public int PokemonApiId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string PrimaryType { get; set; } = string.Empty;

        public string? SecondaryType { get; set; }

        public int Position { get; set; }

        public int Generation { get; set;  }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }//Propiedad en caso de usar EF
    }
}
