using GameLibrary.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace GameLibrary.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly GameContext _context;

        public GamesController(GameContext context)
        {
            _context = context;
        }

        // GET: api/games
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var games = await _context.Games.ToListAsync();
            return Ok(games);
        }

        // GET: api/games/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null)
                return NotFound();

            return Ok(game);
        }

        // GET: api/games/owner/1
        [HttpGet("owner/{ownerId}")]
        public async Task<IActionResult> GetByOwnerId(int ownerId)
        {
            var games = await _context.Games
                .Where(g => g.OwnerId == ownerId)
                .ToListAsync();

            return Ok(games);
        }


        // POST: api/games
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Game game)
        {
            // Simulando um usuário fixo (ex: ID = 1)
            game.OwnerId = 1;

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByIdAsync), new { id = game.Id }, game);
        }

        // PUT: api/games/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Game updatedGame)
        {
            var existingGame = await _context.Games.FindAsync(id);

            if (existingGame == null)
                return NotFound();

            // Atualiza apenas os campos que podem ser modificados
            existingGame.Name = updatedGame.Name;
            existingGame.Studio = updatedGame.Studio;
            existingGame.CoverImageUrl = updatedGame.CoverImageUrl;
            existingGame.Price = updatedGame.Price;
            existingGame.Description = updatedGame.Description;
            existingGame.SteamLink = updatedGame.SteamLink;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/games/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var game = await _context.Games.FindAsync(id);

            if (game == null)
                return NotFound();

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}