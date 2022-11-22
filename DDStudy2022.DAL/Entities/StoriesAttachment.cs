using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.DAL.Entities
{
    public class StoriesAttachment : Attachment
    {
        public Guid StoriesId { get; set; }
        public Stories Stories { get; set; } = null!;
    }
}
