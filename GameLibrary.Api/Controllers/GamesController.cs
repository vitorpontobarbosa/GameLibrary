using GameLibrary.Api.DTOs.Games;
using GameLibrary.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameLibrary.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var games = await _gameService.GetAllAsync();
            return Ok(games);
        }

        [HttpGet("{id}", Name = "GetGameById")]
        public async Task<IActionResult> GetGameById(int id)
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(userIdValue, out var userId);
            var game = await _gameService.GetByIdAsync(id, userId);
            if (game == null)
                return NotFound();
            return Ok(game);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyGames()
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(userIdValue, out var userId);
            var games = await _gameService.GetMyGamesAsync(userId);
            return Ok(games);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateGameRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(userIdValue, out var userId);
            var game = await _gameService.CreateAsync(request, userId);
            return CreatedAtRoute("GetGameById", new { id = game.Id }, game);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateGameRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(userIdValue, out var userId);
            var updated = await _gameService.UpdateAsync(id, request, userId);
            if (!updated)
                return Forbid();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(userIdValue, out var userId);
            var deleted = await _gameService.DeleteAsync(id, userId);
            if (!deleted)
                return Forbid();
            return NoContent();
        }
    }
}