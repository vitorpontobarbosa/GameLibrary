using GameLibrary.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameLibrary.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        // Lista em memória simulando um "banco de dados"
        private static List<Game> games = new();

        // GET: api/games
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(games);
        }

        // GET: api/games/1
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var game = games.FirstOrDefault(g => g.Id == id);
            if (game == null)
                return NotFound();

            return Ok(game);
        }

        // POST: api/games
        [HttpPost]
        public IActionResult Create(Game game)
        {
            game.Id = games.Count > 0 ? games.Max(g => g.Id) + 1 : 1;
            games.Add(game);
            return CreatedAtAction(nameof(GetById), new { id = game.Id }, game);
        }

        // PUT: api/games/1
        [HttpPut("{id}")]
        public IActionResult Update(int id, Game updatedGame)
        {
            var index = games.FindIndex(g => g.Id == id);
            if (index == -1)
                return NotFound();

            updatedGame.Id = id;
            games[index] = updatedGame;
            return NoContent();
        }

        // DELETE: api/games/1
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var game = games.FirstOrDefault(g => g.Id == id);
            if (game == null)
                return NotFound();

            games.Remove(game);
            return NoContent();
        }
    }
}
