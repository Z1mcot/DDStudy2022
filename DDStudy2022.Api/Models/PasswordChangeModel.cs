using System.ComponentModel.DataAnnotations;

namespace DDStudy2022.Api.Models
{
    public class PasswordChangeModel
    {
        [Required]
        public string OldPassword { get; set; } = null!;
        [Required]
        public string NewPassword { get; set; } = null!;
        [Required]
        [Compare(nameof(NewPassword))]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
