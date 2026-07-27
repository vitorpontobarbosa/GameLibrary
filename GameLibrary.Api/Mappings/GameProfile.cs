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
            CreateMap<Game, GameDetailResponse>();
            CreateMap<Game, MyGameResponse>();
            CreateMap<CreateGameRequest, Game>();
            CreateMap<UpdateGameRequest, Game>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Game, GameRequest>();
        }
    }
}
