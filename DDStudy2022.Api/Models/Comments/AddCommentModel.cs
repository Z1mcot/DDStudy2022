using DDStudy2022.Api.Models.Attachments;
using System.ComponentModel.DataAnnotations;

namespace DDStudy2022.Api.Models.Comments
{
    public class AddCommentModel
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public Guid PostId { get; set; }
        public string Content { get; set; } = null!;

    }
}
