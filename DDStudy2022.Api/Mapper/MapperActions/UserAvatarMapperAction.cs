using AutoMapper;
using DDStudy2022.Api.Models.Attachments;
using DDStudy2022.Api.Models.Users;
using DDStudy2022.Api.Services;
using DDStudy2022.DAL.Entities;

namespace DDStudy2022.Api.Mapper.MapperActions
{
    public class UserAvatarMapperAction : IMappingAction<User, UserAvatarModel>
    {
        private LinkGeneratorService _linkGenerator;
        public UserAvatarMapperAction(LinkGeneratorService linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        public void Process(User source, UserAvatarModel destination, ResolutionContext context) 
            => _linkGenerator.FixAvatar(source, destination);
    }
}
