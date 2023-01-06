using AutoMapper;
using DDStudy2022.Api.Mapper.MapperActions;
using DDStudy2022.Api.Models;
using DDStudy2022.Api.Models.Attachments;
using DDStudy2022.Api.Models.Comments;
using DDStudy2022.Api.Models.Likes;
using DDStudy2022.Api.Models.Posts;
using DDStudy2022.Api.Models.Sessions;
using DDStudy2022.Api.Models.Stories;
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
                .ForMember(dest => dest.IsActive, map => map.MapFrom(src => true))
                .ForMember(dest => dest.NameTag, map => map.MapFrom(src => $"@{src.NameTag}"));

            CreateMap<User, UserModel>();
            CreateMap<User, UserAvatarModel>()
                .AfterMap<UserAvatarMapperAction>();
            CreateMap<User, UserProfileModel>()
                .ForMember(dest => dest.SubscribersCount, map => map.MapFrom(src => src.Subscribers == null ? 0 : src.Subscribers.Count))
                .ForMember(dest => dest.SubscriptionsCount, map => map.MapFrom(src => src.Subscriptions == null ? 0 : src.Subscriptions.Count))
                .ForMember(dest => dest.PostsCount, map => map.MapFrom(src => src.Posts == null ? 0 : src.Posts.Count))
                .ForMember(dest => dest.isPrivate, map => map.MapFrom(src => src.IsPrivate ? 1 : 0))
                .AfterMap<UserAvatarMapperAction>();

            CreateMap<PasswordChangeRequest, PasswordChangeModel>();
            CreateMap<ModifyUserInfoRequest, ModifyUserInfoModel>();

            // Сессии
            CreateMap<UserSession, SessionModel>();

            // Всё что с аттачами
            CreateMap<Avatar, AttachmentModel>();
            CreateMap<PostAttachment, AttachmentModel>();
            CreateMap<PostAttachment, AttachmentExternalModel>()
                .AfterMap<AttachmentExternalMapperAction>();
            CreateMap<StoriesAttachment, AttachmentModel>();
            CreateMap<StoriesAttachment, AttachmentExternalModel>()
                .AfterMap<StoriesAttachmentMapperAction>();

            // Посты
            CreateMap<CreatePostRequest, CreatePostModel>();
            CreateMap<CreatePostModel, Post>()
                .ForMember(dest => dest.IsShown, map => map.MapFrom(src => true))
                .ForMember(dest => dest.Content, map => map.MapFrom(src => src.Content))
                .ForMember(dest => dest.PublishDate, map => map.MapFrom(src => DateTimeOffset.UtcNow));
            CreateMap<Post, PostModel>()
                .ForMember(dest => dest.Likes, map => map.MapFrom(src => src.Likes == null ? 0 : src.Likes.Count))
                .ForMember(dest => dest.Comments, map => map.MapFrom(src => src.Comments == null ? 0 : src.Comments.Count));
            CreateMap<ModifyPostRequest, ModifyPostModel>();

            // Комменты
            CreateMap<AddCommentModel, PostComment>()
                .ForMember(dest => dest.PublishDate, map => map.MapFrom(src => DateTimeOffset.UtcNow));
            CreateMap<PostComment, CommentModel>()
                .ForMember(dest => dest.Likes, map => map.MapFrom(src => src.Likes == null ? 0 : src.Likes.Count)); ;

            // Метадата
            CreateMap<MetadataModel, MetadataLinkModel>();
            CreateMap<MetadataLinkModel, PostAttachment>();
            CreateMap<MetadataLinkModel, StoriesAttachment>();

            // Подписки
            CreateMap<MakeSubscribtionRequest, UserSubscription>()
                .ForMember(dest => dest.SubscriptionDate, map => map.MapFrom(src => DateTimeOffset.UtcNow));

            // Лайки
            CreateMap<ModifyPostLikeModel, PostLike>();
            CreateMap<PostLike, PostLikeModel>(); 
            CreateMap<ModifyCommentLikeModel, CommentLike>();
            CreateMap<CommentLike, CommentLikeModel>();

            // Сторис
            CreateMap<CreateStoriesRequest, CreateStoriesModel>();
            CreateMap<CreateStoriesModel, Stories>()
                .ForMember(dest => dest.IsShown, map => map.MapFrom(stc => true))
                .ForMember(dest => dest.Content, map => map.MapFrom(src => src.Content))
                .ForMember(dest => dest.PublishDate, map => map.MapFrom(src => DateTimeOffset.UtcNow))
                .ForMember(dest => dest.ExpirationDate, map => map.MapFrom(src => DateTimeOffset.UtcNow.AddDays(1)));
            CreateMap<Stories, StoriesModel>();
        }
    }
}
