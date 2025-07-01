namespace GameLibrary.Api.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Studio { get; set; }
        public string CoverImageUrl { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string SteamLink { get; set; } = string.Empty;
        public int OwnerId { get; set; }
    }
}