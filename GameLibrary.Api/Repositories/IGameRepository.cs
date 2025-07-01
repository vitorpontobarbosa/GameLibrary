using GameLibrary.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameLibrary.Api.Repositories
{
    public interface IGameRepository
    {
        Task<Game?> GetByIdAsync(int id);
        Task<IEnumerable<Game>> GetAllAsync();
        Task<IEnumerable<Game>> GetByOwnerIdAsync(int ownerId);
        Task AddAsync(Game game);
        void Update(Game game);
        void Delete(Game game);
        Task SaveChangesAsync();
    }
}