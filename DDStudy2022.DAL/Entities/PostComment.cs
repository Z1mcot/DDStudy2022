using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.DAL.Entities
{
    public class PostComment
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public virtual User Author { get; set; } = null!;
        public string Content { get; set; } = null!;
        public Guid PostId { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public bool IsModified { get; set; } = false;
    }
}
