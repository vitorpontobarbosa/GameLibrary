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
            // Valida��o: e-mail obrigat�rio e formato
            if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
                throw new ArgumentException("E-mail � obrigat�rio e deve ser v�lido.");
            // Valida��o: senha obrigat�ria e min 6
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
                throw new ArgumentException("Senha � obrigat�ria e deve ter pelo menos 6 caracteres.");

            var exists = await _context.Users.AnyAsync(u => u.Email.ToLower() == request.Email.ToLower());
            if (exists)
                throw new ArgumentException("Usu�rio j� existe.");

            var user = _mapper.Map<User>(request);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new RegisterResponse
            {
                Message = "Usu�rio criado com sucesso!",
                Token = GenerateToken(user)
            };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // Valida��o: e-mail obrigat�rio
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("E-mail � obrigat�rio.");
            // Valida��o: senha obrigat�ria
            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Senha � obrigat�ria.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);
            if (user == null)
                throw new ArgumentException("Usu�rio ou senha inv�lidos.");

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