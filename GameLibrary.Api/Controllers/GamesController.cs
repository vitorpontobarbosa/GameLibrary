using AutoMapper;
using GameLibrary.Api.DTOs.Games;
using GameLibrary.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace GameLibrary.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly GameContext _context;
        private readonly IMapper _mapper;


        public GamesController(GameContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        // GET: api/games
        [HttpGet]
        [ProducesResponseType(typeof(List<Game>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            var games = await _context.Games.ToListAsync();
            return Ok(games);
        }

        // GET: api/games/1
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Game), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
                return NotFound();

            return Ok(game);
        }

        // GET: api/games/me
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(List<Game>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyGames()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var games = await _context.Games
                .Where(g => g.OwnerId == userId)
                .ToListAsync();

            return Ok(games);
        }


        // POST: api/games
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(List<Game>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] CreateGameRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var game = _mapper.Map<Game>(request);
            game.OwnerId = userId;

            if (game.OwnerId != userId)
                return Forbid();

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            var response = _mapper.Map<GameResponse>(game);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = game.Id }, response);
        }


        // PUT: api/games/5
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(List<Game>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateGameRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var existingGame = await _context.Games.FindAsync(id);

            if (existingGame == null)
                return NotFound();

            if (existingGame.OwnerId != userId)
                return Forbid();

            // Atualiza os campos com base no DTO
            _mapper.Map(request, existingGame);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // DELETE: api/games/5
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(List<Game>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var game = await _context.Games.FindAsync(id);
            if (game == null)
                return NotFound();

            if (game.OwnerId != userId)
                return Forbid();

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}