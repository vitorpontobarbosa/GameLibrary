using System.ComponentModel.DataAnnotations;

namespace GameLibrary.Api.DTOs.Games
{
    public class CreateGameRequest
    {
        [Required(ErrorMessage = "O nome do jogo � obrigat�rio.")]
        [MinLength(2, ErrorMessage = "O nome deve ter pelo menos 2 caracteres.")]
        public string Name { get; set; }

        public string? Studio { get; set; }

        [Url(ErrorMessage = "URL da capa inv�lida.")]
        public string? CoverImageUrl { get; set; }

        [Range(0, 1000, ErrorMessage = "O pre�o deve estar entre 0 e 1000.")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "A descri��o � obrigat�ria.")]
        [MinLength(5, ErrorMessage = "A descri��o deve ter pelo menos 5 caracteres.")]
        public string Description { get; set; }

        [Url(ErrorMessage = "Link da Steam inv�lido.")]
        public string SteamLink { get; set; }
    }
}