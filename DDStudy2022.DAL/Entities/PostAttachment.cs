using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.DAL.Entities
{
    public class PostAttachment : Attachment
    {
        public virtual Post Post { get; set; } = null!;
    }
}
