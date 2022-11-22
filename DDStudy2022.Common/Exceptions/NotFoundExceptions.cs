using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public string? Model { get; set; }

        public override string Message => $"{Model} not found";
    }


    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException()
        {
            Model = "User";
        }
    }

    public class PostNotFoundException : NotFoundException
    {
        public PostNotFoundException()
        {
            Model = "Post";
        }
    }

    public class CommentNotFoundException : NotFoundException
    {
        public CommentNotFoundException()
        {
            Model = "Comment";
        }
    }

    public class SubscriptionNotFoundException : NotFoundException
    {
        public SubscriptionNotFoundException()
        {
            Model = "Subscription";
        }
    }

    public class SubscriptionRequestNotFoundException : NotFoundException
    {
        public SubscriptionRequestNotFoundException()
        {
            Model = "Subscription request";
        }
    }

    public class SessionNotFoundException : NotFoundException
    {
        public SessionNotFoundException()
        {
            Model = "Session(-s)";
        }
    }

    public class AttachmentNotFoundException: NotFoundException
    {
        public AttachmentNotFoundException()
        {
            Model = "Attachment(-s)";
        }
    }

    public class TempDirNotFoundException : NotFoundException
    {
        public TempDirNotFoundException()
        {
            Model = "Temp directory";
        }
    }

    public class TempFileNotFoundException : NotFoundException
    {
        public TempFileNotFoundException()
        {
            Model = "Temp file";
        }
    }

    public class LikeNotFoundException : NotFoundException
    {
        public LikeNotFoundException()
        {
            Model = "Like";
        }
    }

    public class StoriesNotFoundException: NotFoundException
    {
        public StoriesNotFoundException()
        {
            Model = "Story";
        }
    }
}

