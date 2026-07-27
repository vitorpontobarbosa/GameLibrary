using System;

namespace GameLibrary.Api.DTOs.Games
{
    public class GameOwnerResponse
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public decimal? PricePaid { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
