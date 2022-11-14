using DDStudy2022.Api.Models.Attachments;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DDStudy2022.Api.Models.Posts
{
    public class CreatePostRequest
    {
        [Required]
        public List<MetadataModel> Content { get; set; } = null!;
        public string? Description { get; set; }
        public Guid? AuthorId { get; set; }
    }
}
