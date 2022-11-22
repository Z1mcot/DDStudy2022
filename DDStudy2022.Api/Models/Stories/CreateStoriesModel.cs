using DDStudy2022.Api.Models.Attachments;

namespace DDStudy2022.Api.Models.Stories
{
    public class CreateStoriesModel
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public MetadataLinkModel Content { get; set; } = null!;
    }
}
