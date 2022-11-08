using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.DAL.Entities
{
    public class Post
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public virtual User Author { get; set; } = null!;
        public virtual IList<PostAttachment> Content { get; set; } = null!;
        public string? Description { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public virtual IList<PostComment>? Comments { get; set; }
        public bool IsShown { get; set; }
        public bool IsModified { get; set; } = false;
        
    }
}
