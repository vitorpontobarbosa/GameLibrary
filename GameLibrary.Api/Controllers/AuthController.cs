using AutoMapper;
using GameLibrary.Api.DTOs.Auth;
using GameLibrary.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GameLibrary.Api.Controllers
{
    /// <summary>
    /// Endpoints relacionados ao usuario.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly GameContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthController(GameContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        /// <summary>Registra Usuario</summary>
        /// <response code="200">Usuário criado com sucesso</response>
        /// <response code="400">Dados inválidos ou usuário já existe</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (exists)
                return BadRequest("Usuário já existe.");

            var user = _mapper.Map<User>(request);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var response = new RegisterResponse
            {
                Message = "Usuário criado com sucesso!",
                Token = GenerateToken(user)
            };

            return Ok(response);
        }

        ///<summary>Realiza o login do usuário</summary>
        ///<response code="200">Login realizado com sucesso</response>
        ///<response code="401">Usuário ou senha inválidos</response>
        ///<response code="400">Dados inválidos</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);

            if (user == null)
                return Unauthorized("Usuário ou senha inválidos.");

            var response = new LoginResponse { 
                Message = "Login realizado com sucesso!",
                Token = GenerateToken(user)

            };
            
            return Ok(response);
        }

        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                 new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                 new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}