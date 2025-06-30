using System.Threading.Tasks;
using GameLibrary.Api.DTOs.Auth;

namespace GameLibrary.Api.Services
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}