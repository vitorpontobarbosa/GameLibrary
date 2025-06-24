using AutoMapper;
using GameLibrary.Api.DTOs.Auth;
using GameLibrary.Api.Models;

namespace GameLibrary.Api.Mappings
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            // Registro
            CreateMap<RegisterRequest, User>();
            CreateMap<User, RegisterResponse>();

            // Login
            CreateMap<User, LoginResponse>();
        }
    }
}
