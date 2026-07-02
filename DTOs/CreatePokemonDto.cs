namespace MyPokemonRankingApi.DTOs
{
    public class CreatePokemonDto //Clase para lo que recibimos del cliente al crear un nuevo Pokémon
    {
        public int PokemonApiId { get; set; }
        public int Position { get; set; }
    }
}
