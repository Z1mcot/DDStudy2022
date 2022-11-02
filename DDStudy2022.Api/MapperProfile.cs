﻿using AutoMapper;
using DDStudy2022.Api.Models.Sessions;
using DDStudy2022.Api.Models.Users;
using DDStudy2022.Common;

namespace DDStudy2022.Api
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CreateUserModel, DAL.Entities.User>()
                .ForMember(dest => dest.Id, map => map.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.PasswordHash, map => map.MapFrom(src => HashHelper.GetHash(src.Password)))
                .ForMember(dest => dest.BirthDate, map => map.MapFrom(src => src.BirthDate.UtcDateTime))
                ;
            CreateMap<DAL.Entities.User, UserModel>();
            CreateMap<DAL.Entities.UserSession, SessionModel>();
        }
    }
}
