using AutoMapper;
using GameLibrary.Api.DTOs.Games;
using GameLibrary.Api.Models;
using GameLibrary.Api.Repositories;
using GameLibrary.Api.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GameLibrary.Tests.Services
{
    public class GameServiceTests
    {
        private readonly Mock<IGameRepository> _gameRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly IGameService _gameService;

        public GameServiceTests()
        {
            _gameRepositoryMock = new Mock<IGameRepository>();
            _mapperMock = new Mock<IMapper>();
            _gameService = new GameService(_gameRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnGameWithOwners_WhenGameExists()
        {
            // Arrange
            var gameId = 1;
            var ownerId = 1;
            var game = new Game { Id = gameId, CreatedByUserId = ownerId };
            var detailResponse = new GameDetailResponse { Id = gameId };
            var ownerships = new List<GameOwnership>
            {
                new GameOwnership { GameId = gameId, UserId = ownerId, PricePaid = 10, User = new User { Id = ownerId, Email = "owner@test.com" } }
            };

            _gameRepositoryMock.Setup(repo => repo.GetByIdAsync(gameId)).ReturnsAsync(game);
            _gameRepositoryMock.Setup(repo => repo.GetOwnersByGameIdAsync(gameId)).ReturnsAsync(ownerships);
            _mapperMock.Setup(mapper => mapper.Map<GameDetailResponse>(game)).Returns(detailResponse);

            // Act
            var result = await _gameService.GetByIdAsync(gameId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(gameId, result.Id);
            Assert.Single(result.Owners);
            Assert.Equal("owner@test.com", result.Owners[0].Email);
            Assert.Equal(10, result.Owners[0].PricePaid);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = 1;

            _gameRepositoryMock.Setup(repo => repo.GetByIdAsync(gameId)).ReturnsAsync((Game?)null);

            // Act
            var result = await _gameService.GetByIdAsync(gameId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllGames()
        {
            // Arrange
            var games = new List<Game>
            {
                new Game { Id = 1, Name = "Game 1" },
                new Game { Id = 2, Name = "Game 2" }
            };
            var gameResponses = new List<GameResponse>
            {
                new GameResponse { Id = 1, Name = "Game 1" },
                new GameResponse { Id = 2, Name = "Game 2" }
            };
            _gameRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(games);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<GameResponse>>(games)).Returns(gameResponses);

            // Act
            var result = await _gameService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetMyGamesAsync_ShouldReturnOwnedGames_WithPricePaid()
        {
            // Arrange
            var userId = 1;
            var game = new Game { Id = 1, Name = "Game 1" };
            var ownerships = new List<GameOwnership>
            {
                new GameOwnership { GameId = 1, UserId = userId, PricePaid = 25, Game = game }
            };
            var myGameResponse = new MyGameResponse { Id = 1, Name = "Game 1" };

            _gameRepositoryMock.Setup(repo => repo.GetOwnedGamesByUserIdAsync(userId)).ReturnsAsync(ownerships);
            _mapperMock.Setup(mapper => mapper.Map<MyGameResponse>(game)).Returns(myGameResponse);

            // Act
            var result = await _gameService.GetMyGamesAsync(userId);

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal(25, resultList[0].PricePaid);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenNameIsNullOrShort()
        {
            // Arrange
            var request = new CreateGameRequest { Name = "", CoverImageUrl = "http://img", Description = "Descrição válida" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _gameService.CreateAsync(request, 1));
            request.Name = "ab";
            await Assert.ThrowsAsync<ArgumentException>(() => _gameService.CreateAsync(request, 1));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenCoverImageUrlIsNullOrEmpty()
        {
            // Arrange
            var request = new CreateGameRequest { Name = "Game", CoverImageUrl = null, Description = "Descrição válida" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _gameService.CreateAsync(request, 1));
            request.CoverImageUrl = "";
            await Assert.ThrowsAsync<ArgumentException>(() => _gameService.CreateAsync(request, 1));
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenDescriptionIsNullOrShort()
        {
            // Arrange
            var request = new CreateGameRequest { Name = "Game", CoverImageUrl = "http://img", Description = null };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _gameService.CreateAsync(request, 1));
            request.Description = "123456789";
            await Assert.ThrowsAsync<ArgumentException>(() => _gameService.CreateAsync(request, 1));
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateOwnership_ForCreator()
        {
            // Arrange
            var userId = 1;
            var request = new CreateGameRequest
            {
                Name = "Game",
                CoverImageUrl = "http://img",
                Description = "Descrição válida",
                Price = 15
            };
            var game = new Game { Id = 1 };
            _mapperMock.Setup(mapper => mapper.Map<Game>(request)).Returns(game);
            _mapperMock.Setup(mapper => mapper.Map<GameResponse>(game)).Returns(new GameResponse { Id = 1 });

            // Act
            await _gameService.CreateAsync(request, userId);

            // Assert
            _gameRepositoryMock.Verify(repo => repo.AddOwnershipAsync(It.Is<GameOwnership>(
                o => o.GameId == game.Id && o.UserId == userId && o.PricePaid == 15)), Times.Once);
        }

        [Fact]
        public async Task AddToCollectionAsync_ShouldThrow_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = 1;
            _gameRepositoryMock.Setup(repo => repo.GetByIdAsync(gameId)).ReturnsAsync((Game?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _gameService.AddToCollectionAsync(gameId, new AddToCollectionRequest(), 1));
        }

        [Fact]
        public async Task AddToCollectionAsync_ShouldThrow_WhenAlreadyOwned()
        {
            // Arrange
            var gameId = 1;
            var userId = 1;
            var game = new Game { Id = gameId };
            _gameRepositoryMock.Setup(repo => repo.GetByIdAsync(gameId)).ReturnsAsync(game);
            _gameRepositoryMock.Setup(repo => repo.GetOwnershipAsync(gameId, userId))
                .ReturnsAsync(new GameOwnership { GameId = gameId, UserId = userId });

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _gameService.AddToCollectionAsync(gameId, new AddToCollectionRequest(), userId));
        }

        [Fact]
        public async Task AddToCollectionAsync_ShouldAddOwnership_WhenNotOwned()
        {
            // Arrange
            var gameId = 1;
            var userId = 2;
            var game = new Game { Id = gameId };
            var request = new AddToCollectionRequest { PricePaid = 42 };
            _gameRepositoryMock.Setup(repo => repo.GetByIdAsync(gameId)).ReturnsAsync(game);
            _gameRepositoryMock.Setup(repo => repo.GetOwnershipAsync(gameId, userId)).ReturnsAsync((GameOwnership?)null);
            _mapperMock.Setup(mapper => mapper.Map<GameResponse>(game)).Returns(new GameResponse { Id = gameId });

            // Act
            await _gameService.AddToCollectionAsync(gameId, request, userId);

            // Assert
            _gameRepositoryMock.Verify(repo => repo.AddOwnershipAsync(It.Is<GameOwnership>(
                o => o.GameId == gameId && o.UserId == userId && o.PricePaid == 42)), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenCallerIsNotCreator()
        {
            // Arrange
            var id = 1;
            var game = new Game { Id = id, CreatedByUserId = 1 };
            _gameRepositoryMock.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(game);

            // Act
            var result = await _gameService.UpdateAsync(id, new UpdateGameRequest(), 2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenNameIsShort()
        {
            // Arrange
            var id = 1;
            var userId = 1;
            var game = new Game { Id = id, CreatedByUserId = userId };
            _gameRepositoryMock.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(game);
            var request = new UpdateGameRequest { Name = "ab" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _gameService.UpdateAsync(id, request, userId));
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenCoverImageUrlIsEmpty()
        {
            // Arrange
            var id = 1;
            var userId = 1;
            var game = new Game { Id = id, CreatedByUserId = userId };
            _gameRepositoryMock.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(game);
            var request = new UpdateGameRequest { CoverImageUrl = "" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _gameService.UpdateAsync(id, request, userId));
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenDescriptionIsShort()
        {
            // Arrange
            var id = 1;
            var userId = 1;
            var game = new Game { Id = id, CreatedByUserId = userId };
            _gameRepositoryMock.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(game);
            var request = new UpdateGameRequest { Description = "123456789" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _gameService.UpdateAsync(id, request, userId));
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenCallerDoesNotOwnGame()
        {
            // Arrange
            var id = 1;
            var userId = 1;
            _gameRepositoryMock.Setup(repo => repo.GetOwnershipAsync(id, userId)).ReturnsAsync((GameOwnership?)null);

            // Act
            var result = await _gameService.DeleteAsync(id, userId);

            // Assert
            Assert.False(result);
            _gameRepositoryMock.Verify(repo => repo.RemoveOwnership(It.IsAny<GameOwnership>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ShouldOnlyRemoveOwnership_WhenOtherOwnersRemain()
        {
            // Arrange
            var id = 1;
            var userId = 1;
            var ownership = new GameOwnership { GameId = id, UserId = userId };
            _gameRepositoryMock.Setup(repo => repo.GetOwnershipAsync(id, userId)).ReturnsAsync(ownership);
            _gameRepositoryMock.Setup(repo => repo.CountOwnersAsync(id)).ReturnsAsync(1);

            // Act
            var result = await _gameService.DeleteAsync(id, userId);

            // Assert
            Assert.True(result);
            _gameRepositoryMock.Verify(repo => repo.RemoveOwnership(ownership), Times.Once);
            _gameRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Game>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ShouldAlsoDeleteGame_WhenLastOwnerRemoved()
        {
            // Arrange
            var id = 1;
            var userId = 1;
            var ownership = new GameOwnership { GameId = id, UserId = userId };
            var game = new Game { Id = id };
            _gameRepositoryMock.Setup(repo => repo.GetOwnershipAsync(id, userId)).ReturnsAsync(ownership);
            _gameRepositoryMock.Setup(repo => repo.CountOwnersAsync(id)).ReturnsAsync(0);
            _gameRepositoryMock.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(game);

            // Act
            var result = await _gameService.DeleteAsync(id, userId);

            // Assert
            Assert.True(result);
            _gameRepositoryMock.Verify(repo => repo.RemoveOwnership(ownership), Times.Once);
            _gameRepositoryMock.Verify(repo => repo.Delete(game), Times.Once);
        }
    }
}
