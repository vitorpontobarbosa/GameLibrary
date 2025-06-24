using System.ComponentModel.DataAnnotations;

namespace GameLibrary.Api.DTOs.Games
{
    public class GameResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Studio { get; set; }
        public string? CoverImageUrl { get; set; }
        public decimal? Price { get; set; }
        public string Description { get; set; }
        public string SteamLink { get; set; }
        public int OwnerId { get; set; }
    }
}
