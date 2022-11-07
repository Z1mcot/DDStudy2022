using AutoMapper;
using DDStudy2022.Api.Models;
using DDStudy2022.Api.Models.Sessions;
using DDStudy2022.Api.Models.Users;
using DDStudy2022.Common;
using System;

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
            CreateMap<DAL.Entities.Avatar, AttachmentModel>();
            CreateMap<DAL.Entities.PostImage, AttachmentModel>();
            CreateMap<DAL.Entities.Post, PostModel>()
                .ForMember(d => d.AttachmentLinks, m => m.MapFrom(s => LinkHelper.GetLinksOfAttachments(s.Content)));
            CreateMap<DAL.Entities.PostComment, CommentModel>()
                .ForMember(d => d.Author, m => m.MapFrom(s => s.Author.Name));
            CreateMap<AddCommentModel, DAL.Entities.PostComment>();
        }
    }
}
