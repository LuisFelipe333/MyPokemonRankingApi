namespace MyPokemonRankingApi.DTOs
{
    public class PokemonDto //Dto para no exponer la entidad completa al cliente, solo los datos necesarios
    {
        public int Id { get; set; }
        public int PokemonApiId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PrimaryType { get; set; } = string.Empty;
        public string? SecondaryType { get; set; }
        public int Position { get; set; }
        public int Generation { get; set; }
    }
}
