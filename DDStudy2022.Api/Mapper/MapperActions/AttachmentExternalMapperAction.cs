using AutoMapper;
using DDStudy2022.Api.Models.Attachments;
using DDStudy2022.Api.Services;
using DDStudy2022.DAL.Entities;

namespace DDStudy2022.Api.Mapper.MapperActions
{
    public class AttachmentExternalMapperAction : IMappingAction<PostAttachment, AttachmentExternalModel>
    {


        private LinkGeneratorService _linkGenerator;
        public AttachmentExternalMapperAction(LinkGeneratorService linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        public void Process(PostAttachment source, AttachmentExternalModel destination, ResolutionContext context)
            => _linkGenerator.FixContent(source, destination);
    }
}
