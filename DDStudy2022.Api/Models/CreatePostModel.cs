using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DDStudy2022.Api.Models
{
    public class CreatePostModel
    {
        [FromForm]
        [Required]
        public List<IFormFile> Attachments { get; set; } = null!;
        public string? Description { get; set; }
    }
}
