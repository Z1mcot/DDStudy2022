using DDStudy2022.Api.Models.Attachments;

namespace DDStudy2022.Api.Models.Posts
{
    public class ModifyPostModel
    {
        public Guid AuthorId { get; set; }
        public string? Description { get; set; }
        public List<MetadataLinkModel> Content { get; set; } = null!;
    }
}
