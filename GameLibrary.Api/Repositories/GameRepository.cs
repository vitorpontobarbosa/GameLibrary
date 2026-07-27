using GameLibrary.Api.Data;
using GameLibrary.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibrary.Api.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly GameContext _context;
        public GameRepository(GameContext context)
        {
            _context = context;
        }

        public async Task<Game?> GetByIdAsync(int id)
        {
            return await _context.Games.FindAsync(id);
        }

        public async Task<IEnumerable<Game>> GetAllAsync()
        {
            return await _context.Games.ToListAsync();
        }

        public async Task AddAsync(Game game)
        {
            await _context.Games.AddAsync(game);
        }

        public void Update(Game game)
        {
            _context.Games.Update(game);
        }

        public void Delete(Game game)
        {
            _context.Games.Remove(game);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<GameOwnership?> GetOwnershipAsync(int gameId, int userId)
        {
            return await _context.GameOwnerships
                .FirstOrDefaultAsync(o => o.GameId == gameId && o.UserId == userId);
        }

        public async Task<IEnumerable<GameOwnership>> GetOwnersByGameIdAsync(int gameId)
        {
            return await _context.GameOwnerships
                .Where(o => o.GameId == gameId)
                .Include(o => o.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<GameOwnership>> GetOwnedGamesByUserIdAsync(int userId)
        {
            return await _context.GameOwnerships
                .Where(o => o.UserId == userId)
                .Include(o => o.Game)
                .ToListAsync();
        }

        public async Task AddOwnershipAsync(GameOwnership ownership)
        {
            await _context.GameOwnerships.AddAsync(ownership);
        }

        public void RemoveOwnership(GameOwnership ownership)
        {
            _context.GameOwnerships.Remove(ownership);
        }

        public async Task<int> CountOwnersAsync(int gameId)
        {
            return await _context.GameOwnerships.CountAsync(o => o.GameId == gameId);
        }
    }
}
