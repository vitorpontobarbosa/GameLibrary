using GameLibrary.Api.Data;
using GameLibrary.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        public async Task<IEnumerable<Game>> GetByOwnerIdAsync(int ownerId)
        {
            return await _context.Games.Where(g => g.OwnerId == ownerId).ToListAsync();
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
    }
}