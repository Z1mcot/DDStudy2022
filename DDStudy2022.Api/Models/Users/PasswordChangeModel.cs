﻿using System.ComponentModel.DataAnnotations;

namespace DDStudy2022.Api.Models.Users
{
    public class PasswordChangeModel
    {
        public Guid Id { get; set; }
        [Required]
        public string OldPassword { get; set; } = null!;
        [Required]
        public string NewPassword { get; set; } = null!;
        [Required]
        [Compare(nameof(NewPassword))]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
