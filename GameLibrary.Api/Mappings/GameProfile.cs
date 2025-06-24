using AutoMapper;
using GameLibrary.Api.DTOs.Games;
using GameLibrary.Api.Models;

namespace GameLibrary.Api.Mappings
{
    public class GameProfile : Profile
    {
        public GameProfile()
        {
            CreateMap<Game, GameResponse>();
            CreateMap<CreateGameRequest, Game>();
            CreateMap<UpdateGameRequest, Game>();
            CreateMap<Game, GameRequest>();
        }
    }
}
