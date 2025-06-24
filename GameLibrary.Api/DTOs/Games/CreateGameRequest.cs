using System.ComponentModel.DataAnnotations;

namespace GameLibrary.Api.DTOs.Games
{
    public class CreateGameRequest
    {
        [Required]
        public string Name { get; set; }
        public string? Studio { get; set; }
        public string? CoverImageUrl { get; set; }
        public decimal? Price { get; set; }
        [Required]
        [MaxLength(500, ErrorMessage = "A descrição não pode ter mais de 500 caracteres.")]
        public string Description { get; set; }
        public string SteamLink { get; set; }
    }
}