using DDStudy2022.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.Common
{
    public static class LinkHelper
    {
        public static string GetLinkOfAttachment(PostImage im)
            => $"api/Attachment/ShowAttachment?attachmentId={im.Id}";

        public static List<string> GetLinksOfAttachments(IList<PostImage> ims)
            => ims.Select(x => GetLinkOfAttachment(x)).ToList();
    }
}
