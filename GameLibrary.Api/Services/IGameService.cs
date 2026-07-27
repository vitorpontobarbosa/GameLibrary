using GameLibrary.Api.DTOs.Games;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameLibrary.Api.Services
{
    public interface IGameService
    {
        Task<GameDetailResponse?> GetByIdAsync(int id);
        Task<IEnumerable<GameResponse>> GetAllAsync();
        Task<IEnumerable<MyGameResponse>> GetMyGamesAsync(int userId);
        Task<GameResponse> CreateAsync(CreateGameRequest request, int userId);
        Task<bool> UpdateAsync(int id, UpdateGameRequest request, int userId);
        Task<bool> DeleteAsync(int id, int userId);
        Task<GameResponse> AddToCollectionAsync(int gameId, AddToCollectionRequest request, int userId);
    }
}
