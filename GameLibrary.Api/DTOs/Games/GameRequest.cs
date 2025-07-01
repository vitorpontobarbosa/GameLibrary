using System.ComponentModel.DataAnnotations;

namespace GameLibrary.Api.DTOs.Games
{
    public class GameRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Studio { get; set; }

        public string? CoverImageUrl { get; set; }

        public decimal? Price { get; set; }

        [Required]
        [MaxLength(500, ErrorMessage = "A descrição não pode ter mais de 500 caracteres.")]
        public string Description { get; set; } = string.Empty;

        public string SteamLink { get; set; } = string.Empty;
    }
}
