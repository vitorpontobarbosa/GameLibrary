using AutoMapper;
using GameLibrary.Api.DTOs.Games;
using GameLibrary.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace GameLibrary.Api.Controllers
{
    /// <summary>
    /// Endpoints relacionados aos jogos da biblioteca.
    /// </summary>
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
        /// <summary>Retorna todos os jogos registrados</summary>
        /// <response code="200">Lista de jogos retornada com sucesso</response>
        /// <response code ="404">Nenhum jogo encontrado</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<Game>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            var games = await _context.Games.ToListAsync();
            return Ok(games);
        }

        // GET: api/games/1
        /// <summary>Retorna um jogo com base no ID.</summary>
        /// <response code="200">jogo retornado com sucesso</response>
        /// <response code ="404">Nenhum jogo encontrado</response>
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
        /// <summary>Retorna todos os jogos do usuário.</summary>
        /// <response code="200">Lista de jogos retornada com sucesso</response>
        /// <response code ="404">Nenhum jogo encontrado</response>
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
        /// <summary>Cria jogo.</summary>
        /// <response code="200">Jogo Criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(List<Game>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateGameRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var game = _mapper.Map<Game>(request);
            game.OwnerId = userId;

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            var response = _mapper.Map<GameResponse>(game);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = game.Id }, response);
        }


        // PUT: api/games/5
        /// <summary>Atualiza jogo.</summary>
        /// <response code="200">Jogo atualizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="403">Usuário não autorizado</response>
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
        /// <summary>Deleta jogo.</summary>
        /// <response code="204">Jogo deletado com sucesso</response>
        /// <response code="404">Jogo não encontrado</response>
        /// <response code="403">Usuário não autorizado</response>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(List<Game>), StatusCodes.Status204NoContent)]
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