using DDStudy2022.Api.Models.Attachments;

namespace DDStudy2022.Api.Models.Posts
{
    public class CreatePostModel
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public Guid AuthorId { get; set; }
        public List<MetadataWithPathModel> Content { get; set; } = new List<MetadataWithPathModel>();
    }
}
