using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.DAL.Entities
{
    public class Post
    {
        public long Id { get; set; }
        public virtual IList<PostAttachment> Content { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime PublishDate { get; set; }
        public virtual IList<PostComment>? Comments { get; set; }

        
    }
}
