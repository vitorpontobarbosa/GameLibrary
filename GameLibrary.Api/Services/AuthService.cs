using AutoMapper;
using GameLibrary.Api.Data;
using GameLibrary.Api.DTOs.Auth;
using GameLibrary.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly GameContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(GameContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            // Validação: e-mail obrigatório e formato
            if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
                throw new ArgumentException("E-mail é obrigatório e deve ser válido.");
            // Validação: senha obrigatória e min 6
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
                throw new ArgumentException("Senha é obrigatória e deve ter pelo menos 6 caracteres.");

            var exists = await _context.Users.AnyAsync(u => u.Email.ToLower() == request.Email.ToLower());
            if (exists)
                throw new ArgumentException("Usuário já existe.");

            var user = _mapper.Map<User>(request);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new RegisterResponse
            {
                Message = "Usuário criado com sucesso!",
                Token = GenerateToken(user)
            };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // Validação: e-mail obrigatório
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("E-mail é obrigatório.");
            // Validação: senha obrigatória
            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Senha é obrigatória.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);
            if (user == null)
                throw new ArgumentException("Usuário ou senha inválidos.");

            return new LoginResponse
            {
                Message = "Login realizado com sucesso!",
                Token = GenerateToken(user)
            };
        }

        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
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