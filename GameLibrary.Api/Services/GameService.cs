using AutoMapper;
using GameLibrary.Api.DTOs.Games;
using GameLibrary.Api.Models;
using GameLibrary.Api.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameLibrary.Api.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _repository;
        private readonly IMapper _mapper;

        public GameService(IGameRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GameResponse> GetByIdAsync(int id, int userId)
        {
            var game = await _repository.GetByIdAsync(id);
            if (game == null || game.OwnerId != userId)
                return null;
            return _mapper.Map<GameResponse>(game);
        }

        public async Task<IEnumerable<GameResponse>> GetAllAsync()
        {
            var games = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<GameResponse>>(games);
        }

        public async Task<IEnumerable<GameResponse>> GetMyGamesAsync(int userId)
        {
            var games = await _repository.GetByOwnerIdAsync(userId);
            return _mapper.Map<IEnumerable<GameResponse>>(games);
        }

        public async Task<GameResponse> CreateAsync(CreateGameRequest request, int userId)
        {
            var game = _mapper.Map<Game>(request);
            game.OwnerId = userId;
            await _repository.AddAsync(game);
            await _repository.SaveChangesAsync();
            return _mapper.Map<GameResponse>(game);
        }

        public async Task<bool> UpdateAsync(int id, UpdateGameRequest request, int userId)
        {
            var game = await _repository.GetByIdAsync(id);
            if (game == null || game.OwnerId != userId)
                return false;
            _mapper.Map(request, game);
            _repository.Update(game);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var game = await _repository.GetByIdAsync(id);
            if (game == null || game.OwnerId != userId)
                return false;
            _repository.Delete(game);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}