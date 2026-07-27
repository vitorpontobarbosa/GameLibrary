using GameLibrary.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameLibrary.Api.Repositories
{
    public interface IGameRepository
    {
        Task<Game?> GetByIdAsync(int id);
        Task<IEnumerable<Game>> GetAllAsync();
        Task AddAsync(Game game);
        void Update(Game game);
        void Delete(Game game);
        Task SaveChangesAsync();

        Task<GameOwnership?> GetOwnershipAsync(int gameId, int userId);
        Task<IEnumerable<GameOwnership>> GetOwnersByGameIdAsync(int gameId);
        Task<IEnumerable<GameOwnership>> GetOwnedGamesByUserIdAsync(int userId);
        Task AddOwnershipAsync(GameOwnership ownership);
        void RemoveOwnership(GameOwnership ownership);
        Task<int> CountOwnersAsync(int gameId);
    }
}
