﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.DAL.Entities
{
    public class Avatar: Attachment
    {
        public Guid OwnerId { get; set; }
        public virtual User Owner { get; set; } = null!;
    }
}
