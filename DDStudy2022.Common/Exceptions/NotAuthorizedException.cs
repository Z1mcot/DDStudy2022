using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.Common.Exceptions
{
    public class ForbiddenActionException : Exception
    {
        public string? Action { get; set; }

        public override string Message => $"You are not allowed to {Action}";
    }

    public class ModifyPostException : ForbiddenActionException
    {
        public ModifyPostException()
        {
            Action = "modify this post";
        }
    }
    
    public class ModifyCommentException : ForbiddenActionException
    {
        public ModifyCommentException()
        {
            Action = "modify this comment";
        }
    }

    public class DeleteStoryException : ForbiddenActionException
    {
        public DeleteStoryException()
        {
            Action = "delete this story";
        }
    }

    public class PrivateAccountNonsubException : ForbiddenActionException
    {
        public PrivateAccountNonsubException()
        {
            Action = "see this users posts.";
        }
    }

    public class PrivateAccountInfoException : ForbiddenActionException
    {
        public PrivateAccountInfoException()
        {
            Action = "see this user info";
        }
    }
}
