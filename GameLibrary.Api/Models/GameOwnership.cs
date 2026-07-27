using System;

namespace GameLibrary.Api.Models
{
    public class GameOwnership
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public Game? Game { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public decimal? PricePaid { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
