using System.ComponentModel.DataAnnotations;

namespace DDStudy2022.Api.Models.Comments
{
    public class AddCommentRequest
    {
        [Required]
        public Guid PostId { get; set; }
        [Required]
        public string Content { get; set; } = null!;
    }
}
