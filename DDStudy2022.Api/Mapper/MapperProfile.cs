using AutoMapper;
using DDStudy2022.Api.Mapper.MapperActions;
using DDStudy2022.Api.Models;
using DDStudy2022.Api.Models.Attachments;
using DDStudy2022.Api.Models.Comments;
using DDStudy2022.Api.Models.Posts;
using DDStudy2022.Api.Models.Sessions;
using DDStudy2022.Api.Models.Subscriptions;
using DDStudy2022.Api.Models.Users;
using DDStudy2022.Common;
using DDStudy2022.DAL.Entities;
using System;

namespace DDStudy2022.Api.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            // Юзеры
            CreateMap<CreateUserModel, User>()
                .ForMember(dest => dest.Id, map => map.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.PasswordHash, map => map.MapFrom(src => HashHelper.GetHash(src.Password)))
                .ForMember(dest => dest.BirthDate, map => map.MapFrom(src => src.BirthDate.UtcDateTime))
                .ForMember(dest => dest.IsActive, map => map.MapFrom(src => true));

            CreateMap<User, UserModel>();
            CreateMap<User, UserAvatarModel>()
                .AfterMap<UserAvatarMapperAction>();

            // Сессии
            CreateMap<UserSession, SessionModel>();

            // Всё что с аттачами
            CreateMap<Avatar, AttachmentModel>();
            CreateMap<PostAttachment, AttachmentModel>();
            CreateMap<PostAttachment, AttachmentExternalModel>()
                .AfterMap<AttachmentExternalMapperAction>();

            // Посты
            CreateMap<CreatePostRequest, CreatePostModel>();
            CreateMap<CreatePostModel, Post>()
                .ForMember(dest => dest.IsShown, map => map.MapFrom(src => true))
                .ForMember(dest => dest.Content, map => map.MapFrom(src => src.Content))
                .ForMember(dest => dest.PublishDate, map => map.MapFrom(src => DateTimeOffset.UtcNow));
            CreateMap<Post, PostModel>();
            CreateMap<ModifyPostRequest, ModifyPostModel>();

            // Комменты
            CreateMap<AddCommentModel, PostComment>()
                .ForMember(dest => dest.PublishDate, map => map.MapFrom(src => DateTimeOffset.UtcNow));
            CreateMap<PostComment, CommentModel>();

            // Метадата
            CreateMap<MetadataModel, MetadataLinkModel>();
            CreateMap<MetadataLinkModel, PostAttachment>();

            // Подписки
            CreateMap<MakeSubscribtionRequest, UserSubscription>()
                .ForMember(dest => dest.SubscriptionDate, map => map.MapFrom(src => DateTimeOffset.UtcNow));
        }
    }
}
