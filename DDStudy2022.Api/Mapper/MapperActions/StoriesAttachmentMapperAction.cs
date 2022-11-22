using AutoMapper;
using DDStudy2022.Api.Models.Attachments;
using DDStudy2022.Api.Services;
using DDStudy2022.DAL.Entities;

namespace DDStudy2022.Api.Mapper.MapperActions
{
    public class StoriesAttachmentMapperAction : IMappingAction<StoriesAttachment, AttachmentExternalModel>
    {
        private LinkGeneratorService _linkGenerator;
        public StoriesAttachmentMapperAction(LinkGeneratorService linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        public void Process(StoriesAttachment source, AttachmentExternalModel destination, ResolutionContext context)
            => _linkGenerator.FixStories(source, destination);
    }
}
