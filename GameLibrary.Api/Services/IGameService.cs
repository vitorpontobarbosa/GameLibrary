using GameLibrary.Api.DTOs.Games;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameLibrary.Api.Services
{
    public interface IGameService
    {
        Task<GameResponse> GetByIdAsync(int id, int userId);
        Task<IEnumerable<GameResponse>> GetAllAsync();
        Task<IEnumerable<GameResponse>> GetMyGamesAsync(int userId);
        Task<GameResponse> CreateAsync(CreateGameRequest request, int userId);
        Task<bool> UpdateAsync(int id, UpdateGameRequest request, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }
}