using DDStudy2022.Api.Models.Attachments;

namespace DDStudy2022.Api.Models.Posts
{
    public class ModifyPostModel
    {
        public string? Description { get; set; }
        public List<MetadataWithPathModel> Content { get; set; } = null!;
    }
}
