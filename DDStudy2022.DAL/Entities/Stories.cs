using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.DAL.Entities
{
    public class Stories
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public virtual User Author { get; set; } = null!;
        public virtual StoriesAttachment Content { get; set; } = null!;
        public DateTimeOffset PublishDate { get; set; }
        public DateTimeOffset ExpirationDate { get; set; }
        public bool IsShown { get; set; }
    }
}
