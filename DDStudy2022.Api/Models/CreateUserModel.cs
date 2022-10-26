using System.ComponentModel.DataAnnotations;

namespace DDStudy2022.Api.Models
{
    public class CreateUserModel
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        [Compare(nameof(Password))]
        public string? RetryPassword { get; set; }
        [Required]
        public DateTimeOffset BirthDate { get; set; }
    }
}
