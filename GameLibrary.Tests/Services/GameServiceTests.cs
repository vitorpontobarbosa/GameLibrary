using AutoMapper;
using GameLibrary.Api.DTOs.Games;
using GameLibrary.Api.Models;
using GameLibrary.Api.Repositories;
using GameLibrary.Api.Services;
using Moq;


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
        public async Task GetTaskAsync_ShouldReturnGame_WhenGameExists()
        {
            // Arrange
            var gameId = 1;
            var userId = 1;
            var game = new Game { Id = gameId, OwnerId = userId };
            var gameResponse = new GameResponse { Id = gameId };

            _gameRepositoryMock.Setup(repo => repo.GetByIdAsync(gameId)).ReturnsAsync(game);
            _mapperMock.Setup(mapper => mapper.Map<GameResponse>(game)).Returns(gameResponse);

            // Act
            var result = await _gameService.GetByIdAsync(gameId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(gameId, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = 1;
            var userId = 1;
                        
            _gameRepositoryMock.Setup(repo => repo.GetByIdAsync(gameId)).ReturnsAsync((Game?)null);

            // Act
            var result = await _gameService.GetByIdAsync(gameId, userId);

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

    }
}
