using System.ComponentModel.DataAnnotations;

namespace DDStudy2022.Api.Models
{
    public class AddCommentModel
    {
        [Required]
        public long PostId { get; set; }
        [Required]
        public string Content { get; set; } = null!;

    }
}
