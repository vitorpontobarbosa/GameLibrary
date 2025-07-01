namespace GameLibrary.Api.DTOs.Auth
{
    public class RegisterResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}