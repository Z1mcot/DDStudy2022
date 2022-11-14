using AutoMapper;
using DDStudy2022.Api.Models;
using DDStudy2022.Api.Models.Attachments;
using DDStudy2022.Api.Models.Comments;
using DDStudy2022.Api.Models.Posts;
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
                .ForMember(dest => dest.IsActive, map => map.MapFrom(src => true));

            CreateMap<DAL.Entities.User, UserModel>();
            CreateMap<DAL.Entities.User, UserAvatarModel>();

            CreateMap<DAL.Entities.UserSession, SessionModel>();

            CreateMap<DAL.Entities.Avatar, AttachmentModel>();
            CreateMap<DAL.Entities.PostAttachment, AttachmentModel>();
            CreateMap<DAL.Entities.PostAttachment, AttachmentExternalModel>();


            CreateMap<CreatePostRequest, CreatePostModel>();
            CreateMap<CreatePostModel, DAL.Entities.Post>()
                .ForMember(dest => dest.IsShown, map => map.MapFrom(src => true))
                .ForMember(dest => dest.Content, map => map.MapFrom(src => src.Content))
                .ForMember(dest => dest.PublishDate, map => map.MapFrom(src => DateTime.UtcNow));
            CreateMap<ModifyPostRequest, ModifyPostModel>();

            CreateMap<AddCommentModel, DAL.Entities.PostComment>()
                .ForMember(dest => dest.PublishDate, map => map.MapFrom(src => DateTime.UtcNow));

            CreateMap<MetadataModel, MetadataLinkModel>();
            CreateMap<MetadataLinkModel, DAL.Entities.PostAttachment>();

            // Вот здесь надо понормальному смапить User в UserAvatarModel, чтобы не выдавало ошибку
            /*CreateMap<DAL.Entities.PostComment, CommentModel>();*/
        }
    }
}
