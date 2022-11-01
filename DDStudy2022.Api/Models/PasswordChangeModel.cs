using System.ComponentModel.DataAnnotations;

namespace DDStudy2022.Api.Models
{
    public class PasswordChangeModel
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [Compare(nameof(NewPassword))]
        public string ConfirmNewPassword { get; set; }
    }
}
