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
        public string? Model { get; set; }

        public override string Message => $"You are not allowed to {Model}";
    }

    public class ModifyPostException : ForbiddenActionException
    {
        public ModifyPostException()
        {
            Model = "modify this post";
        }
    }
    
    public class ModifyCommentException : ForbiddenActionException
    {
        public ModifyCommentException()
        {
            Model = "modify this comment";
        }
    }

    public class PrivateAccountNonsubException : ForbiddenActionException
    {
        public PrivateAccountNonsubException()
        {
            Model = "see this users posts.";
        }
    }

    public class PrivateAccountInfoException : ForbiddenActionException
    {
        public PrivateAccountInfoException()
        {
            Model = "see this user info";
        }
    }
}
