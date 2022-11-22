using DDStudy2022.Api.Models.Attachments;
using System.ComponentModel.DataAnnotations;

namespace DDStudy2022.Api.Models.Stories
{
    public class CreateStoriesRequest
    {
        [Required]
        public MetadataModel Content { get; set; } = null!;
        public Guid? AuthorId { get; set; }
    }
}
