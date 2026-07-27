using System.ComponentModel.DataAnnotations;

namespace GameLibrary.Api.DTOs.Games
{
    public class AddToCollectionRequest
    {
        [Range(0, 1000, ErrorMessage = "O preço deve estar entre 0 e 1000.")]
        public decimal? PricePaid { get; set; }
    }
}
