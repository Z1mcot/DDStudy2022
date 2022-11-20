using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.DAL.Entities
{
    public class PostLike
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
        public Guid PostId { get; set; }
        public virtual Post Post { get; set; } = null!;
    }
}
