using DDStudy2022.DAL.Entities;

namespace DDStudy2022.Api.Models
{
    public class ModifyPostModel
    {
        public string? Description { get; set; }
        public List<AttachmentModel> Content { get; set; } = null!;
    }
}
