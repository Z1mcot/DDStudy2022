using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.DAL.Entities
{
    public class CommentLike : Like
    {
        public Guid CommentId { get; set; }
        public virtual PostComment Comment { get; set; } = null!;
    }
}
