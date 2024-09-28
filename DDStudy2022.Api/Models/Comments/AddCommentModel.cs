using DDStudy2022.Api.Models.Attachments;
using System.ComponentModel.DataAnnotations;

namespace DDStudy2022.Api.Models.Comments
{
    public class AddCommentModel
    {
        [Required]
        public string Content { get; set; } = null!;

    }
}
