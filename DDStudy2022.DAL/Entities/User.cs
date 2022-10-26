using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.DAL.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "not entered";
        public string Email { get; set; } = "not entered";
        public string PasswordHash { get; set; } = "not entered";
        public DateTimeOffset BirthDate { get; set; }
    }
}
