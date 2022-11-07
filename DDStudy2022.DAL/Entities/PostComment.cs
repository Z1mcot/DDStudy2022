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
        public long Id { get; set; }
        public virtual User Author { get; set; } = null!;
        public virtual Post Post { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime PublishDate { get; set; }
    }
}
