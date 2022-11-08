using System.ComponentModel.DataAnnotations;

namespace DDStudy2022.Api.Models
{
    public class AddCommentModel
    {
        [Required]
        public Guid PostId { get; set; }
        public string Content { get; set; } = null!;

    }
}
