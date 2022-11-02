using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.DAL.Entities
{
    public class UserSession
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RefreshToken { get; set; }
        public DateTime Created { get; set; }
        public bool IsActive { get; set; } = true;
        // На самом деле сессия не может существовать без юзера
        public virtual User? User { get; set; }
    }
}
