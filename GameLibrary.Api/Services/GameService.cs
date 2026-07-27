using AutoMapper;
using GameLibrary.Api.DTOs.Games;
using GameLibrary.Api.Models;
using GameLibrary.Api.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<GameDetailResponse?> GetByIdAsync(int id)
        {
            var game = await _repository.GetByIdAsync(id);
            if (game == null)
                return null;

            var response = _mapper.Map<GameDetailResponse>(game);
            var owners = await _repository.GetOwnersByGameIdAsync(id);
            response.Owners = owners
                .Select(o => new GameOwnerResponse
                {
                    UserId = o.UserId,
                    Email = o.User?.Email ?? string.Empty,
                    PricePaid = o.PricePaid,
                    AddedAt = o.AddedAt
                })
                .ToList();

            return response;
        }

        public async Task<IEnumerable<GameResponse>> GetAllAsync()
        {
            var games = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<GameResponse>>(games);
        }

        public async Task<IEnumerable<MyGameResponse>> GetMyGamesAsync(int userId)
        {
            var ownerships = await _repository.GetOwnedGamesByUserIdAsync(userId);
            return ownerships
                .Where(o => o.Game != null)
                .Select(o =>
                {
                    var response = _mapper.Map<MyGameResponse>(o.Game);
                    response.PricePaid = o.PricePaid;
                    return response;
                })
                .ToList();
        }

        public async Task<GameResponse> CreateAsync(CreateGameRequest request, int userId)
        {
            // Validação: Name obrigatório, min 3
            if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length < 3)
                throw new ArgumentException("O nome do jogo é obrigatório e deve ter pelo menos 3 caracteres.");

            // Validação: CoverImageUrl obrigatório
            if (string.IsNullOrWhiteSpace(request.CoverImageUrl))
                throw new ArgumentException("A URL da capa é obrigatória.");

            // Validação: Description obrigatório, min 10
            if (string.IsNullOrWhiteSpace(request.Description) || request.Description.Length < 10)
                throw new ArgumentException("A descrição é obrigatória e deve ter pelo menos 10 caracteres.");

            var game = _mapper.Map<Game>(request);
            game.CreatedByUserId = userId;
            await _repository.AddAsync(game);
            await _repository.SaveChangesAsync();

            await _repository.AddOwnershipAsync(new GameOwnership
            {
                GameId = game.Id,
                UserId = userId,
                PricePaid = request.Price
            });
            await _repository.SaveChangesAsync();

            return _mapper.Map<GameResponse>(game);
        }

        public async Task<GameResponse> AddToCollectionAsync(int gameId, AddToCollectionRequest request, int userId)
        {
            var game = await _repository.GetByIdAsync(gameId);
            if (game == null)
                throw new KeyNotFoundException("Jogo não encontrado.");

            var existingOwnership = await _repository.GetOwnershipAsync(gameId, userId);
            if (existingOwnership != null)
                throw new ArgumentException("Você já possui este jogo em sua coleção.");

            await _repository.AddOwnershipAsync(new GameOwnership
            {
                GameId = gameId,
                UserId = userId,
                PricePaid = request.PricePaid
            });
            await _repository.SaveChangesAsync();

            return _mapper.Map<GameResponse>(game);
        }

        public async Task<bool> UpdateAsync(int id, UpdateGameRequest request, int userId)
        {
            var game = await _repository.GetByIdAsync(id);
            if (game == null || game.CreatedByUserId != userId)
                return false;

            // Validação: Name se informado, min 3
            if (!string.IsNullOrWhiteSpace(request.Name) && request.Name.Length < 3)
                throw new ArgumentException("O nome do jogo deve ter pelo menos 3 caracteres.");

            // Validação: CoverImageUrl se informado, obrigatório não vazio
            if (request.CoverImageUrl != null && string.IsNullOrWhiteSpace(request.CoverImageUrl))
                throw new ArgumentException("A URL da capa não pode ser vazia se informada.");

            // Validação: Description se informado, min 10
            if (!string.IsNullOrWhiteSpace(request.Description) && request.Description.Length < 10)
                throw new ArgumentException("A descrição deve ter pelo menos 10 caracteres se informada.");

            _mapper.Map(request, game);
            _repository.Update(game);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var ownership = await _repository.GetOwnershipAsync(id, userId);
            if (ownership == null)
                return false;

            _repository.RemoveOwnership(ownership);
            await _repository.SaveChangesAsync();

            var remainingOwners = await _repository.CountOwnersAsync(id);
            if (remainingOwners == 0)
            {
                var game = await _repository.GetByIdAsync(id);
                if (game != null)
                {
                    _repository.Delete(game);
                    await _repository.SaveChangesAsync();
                }
            }

            return true;
        }
    }
}
