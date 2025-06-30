using AutoMapper;
using GameLibrary.Api.Controllers;
using GameLibrary.Api.Data;
using GameLibrary.Api.DTOs.Auth;
using GameLibrary.Api.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace GameLibrary.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<DbSet<User>> _userDbSetMock;
        private readonly Mock<GameContext> _contextMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IConfiguration> _configMock;

        public AuthServiceTests()
        {
            _userDbSetMock = new Mock<DbSet<User>>();
            _contextMock = new Mock<GameContext>(new DbContextOptions<GameContext>());
            _mapperMock = new Mock<IMapper>();
            _configMock = new Mock<IConfiguration>();
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenUserAlreadyExists()
        {
            // Arrange
            var request = new RegisterRequest { Email = "test@email.com", Password = "123456" };
            _contextMock.Setup(c => c.Users).Returns(_userDbSetMock.Object);
            _userDbSetMock
                .Setup(m => m.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(), default))
                .ReturnsAsync(true);

            var controller = new AuthController(
                _contextMock.Object,
                _mapperMock.Object,
                _configMock.Object
            );

            // Act
            var result = await controller.Register(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode ?? 400);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task Register_ShouldReturnOk_WhenUserDoesNotExist()
        {
            // Arrange
            var request = new RegisterRequest { Email = "newuser@email.com", Password = "abcdef" };
            _contextMock.Setup(c => c.Users).Returns(_userDbSetMock.Object);
            _userDbSetMock
                .Setup(m => m.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(), default))
                .ReturnsAsync(false);

            var user = new User { Email = request.Email, Password = request.Password };
            _mapperMock.Setup(m => m.Map<User>(It.IsAny<RegisterRequest>())).Returns(user);

            var users = new List<User>().AsQueryable();
            _userDbSetMock.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            _userDbSetMock.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _userDbSetMock.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _userDbSetMock.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            _userDbSetMock.Setup(m => m.Add(It.IsAny<User>())).Verifiable();
            _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var controller = new AuthController(
                _contextMock.Object,
                _mapperMock.Object,
                _configMock.Object
            );

            // Act
            var result = await controller.Register(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode ?? 200);
            Assert.NotNull(okResult.Value);
            _userDbSetMock.Verify(m => m.Add(It.IsAny<User>()), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenUserDoesNotExist()
        {
            // Arrange
            var request = new LoginRequest { Email = "nouser@email.com", Password = "abcdef" };
            _contextMock.Setup(c => c.Users).Returns(_userDbSetMock.Object);

            _userDbSetMock
                .Setup(m => m.SingleOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(), default))
                .ReturnsAsync((User)null);

            var controller = new AuthController(
                _contextMock.Object,
                _mapperMock.Object,
                _configMock.Object
            );

            // Act
            var result = await controller.Login(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode ?? 401);
            Assert.NotNull(unauthorizedResult.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenUserExistsAndPasswordMatches()
        {
            // Arrange
            var request = new LoginRequest { Email = "user@email.com", Password = "abcdef" };
            var user = new User { Email = request.Email, Password = request.Password };

            _contextMock.Setup(c => c.Users).Returns(_userDbSetMock.Object);
            _userDbSetMock
                .Setup(m => m.SingleOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(), default))
                .ReturnsAsync(user);

            _configMock.Setup(c => c["Jwt:Key"]).Returns("supersecretkeysupersecretkey");
            _configMock.Setup(c => c["Jwt:Issuer"]).Returns("issuer");
            _configMock.Setup(c => c["Jwt:Audience"]).Returns("audience");

            var controller = new AuthController(
                _contextMock.Object,
                _mapperMock.Object,
                _configMock.Object
            );

            // Act
            var result = await controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode ?? 200);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var request = new RegisterRequest { Email = "invalidemail", Password = "123" };
            var controller = new AuthController(
                _contextMock.Object,
                _mapperMock.Object,
                _configMock.Object
            );
            controller.ModelState.AddModelError("Email", "Formato de e-mail inválido.");

            // Act
            var result = await controller.Register(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode ?? 400);
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var request = new LoginRequest { Email = "invalidemail", Password = "123" };
            var controller = new AuthController(
                _contextMock.Object,
                _mapperMock.Object,
                _configMock.Object
            );
            controller.ModelState.AddModelError("Email", "Formato de e-mail inválido.");

            // Act
            var result = await controller.Login(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode ?? 400);
        }
    }
}
