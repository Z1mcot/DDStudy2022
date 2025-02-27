﻿using System.ComponentModel.DataAnnotations;

namespace DDStudy2022.Api.Models.Users
{
    public class CreateUserModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string NameTag { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password))]
        public string RetryPassword { get; set; }
        [Required]
        public DateTimeOffset BirthDate { get; set; }

        public CreateUserModel(string name, string nameTag, string email, string password, string retryPassword, DateTimeOffset birthDate)
        {
            Name = name;
            NameTag = nameTag;
            Email = email;
            Password = password;
            RetryPassword = retryPassword;
            BirthDate = birthDate;
        }
    }
}
