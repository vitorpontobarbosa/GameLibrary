namespace GameLibrary.Api.DTOs.Games
{
    public class MyGameResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Studio { get; set; }
        public string? CoverImageUrl { get; set; }
        public decimal? Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string SteamLink { get; set; } = string.Empty;
        public int CreatedByUserId { get; set; }
        public decimal? PricePaid { get; set; }
    }
}
