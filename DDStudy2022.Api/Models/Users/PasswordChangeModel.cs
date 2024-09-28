using System.ComponentModel.DataAnnotations;

namespace DDStudy2022.Api.Models.Users
{
    [Obsolete("remove all json models that have request classes")]
    public class PasswordChangeModel
    {
        public Guid Id { get; init; }
        [Required]
        public string OldPassword { get; init; } = null!;
        [Required]
        public string NewPassword { get; init; } = null!;
        [Required]
        [Compare(nameof(NewPassword))]
        public string ConfirmNewPassword { get; init; } = null!;
    }
}
