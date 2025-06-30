using AutoMapper;
using GameLibrary.Api.DTOs.Auth;
using GameLibrary.Api.Models;
using GameLibrary.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace GameLibrary.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<DbSet<User>> _userDbSetMock;
        private readonly Mock<GameLibrary.Api.Data.GameContext> _contextMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly IAuthService _authService;

        public AuthServiceTests()
        {
            _userDbSetMock = new Mock<DbSet<User>>();
            _contextMock = new Mock<GameLibrary.Api.Data.GameContext>(new DbContextOptions<GameLibrary.Api.Data.GameContext>());
            _mapperMock = new Mock<IMapper>();
            _configMock = new Mock<IConfiguration>();
            _authService = new AuthService(_contextMock.Object, _mapperMock.Object, _configMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrow_WhenEmailInvalid()
        {
            var request = new RegisterRequest { Email = "", Password = "123456" };
            await Assert.ThrowsAsync<ArgumentException>(() => _authService.RegisterAsync(request));
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrow_WhenPasswordInvalid()
        {
            var request = new RegisterRequest { Email = "test@email.com", Password = "123" };
            await Assert.ThrowsAsync<ArgumentException>(() => _authService.RegisterAsync(request));
        }


        [Fact]
        public async Task LoginAsync_ShouldThrow_WhenEmailEmpty()
        {
            var request = new LoginRequest { Email = "", Password = "abcdef" };
            await Assert.ThrowsAsync<ArgumentException>(() => _authService.LoginAsync(request));
        }

        [Fact]
        public async Task LoginAsync_ShouldThrow_WhenPasswordEmpty()
        {
            var request = new LoginRequest { Email = "user@email.com", Password = "" };
            await Assert.ThrowsAsync<ArgumentException>(() => _authService.LoginAsync(request));
        }
    }
}
