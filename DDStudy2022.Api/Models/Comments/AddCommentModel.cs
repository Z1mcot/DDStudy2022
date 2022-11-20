using DDStudy2022.Api.Models.Attachments;
using System.ComponentModel.DataAnnotations;

namespace DDStudy2022.Api.Models.Comments
{
    public class AddCommentModel
    {
        public Guid? AuthorId { get; set; }
        [Required]
        public Guid PostId { get; set; }
        [Required]
        public string Content { get; set; } = null!;

    }
}
