using DDStudy2022.Api.Models.Attachments;
using DDStudy2022.DAL.Entities;

namespace DDStudy2022.Api.Models.Posts
{
    public class ModifyPostRequest
    {
        public string? Description { get; set; }
        public List<MetadataModel> Content { get; set; } = null!;
    }
}
