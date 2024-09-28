namespace DDStudy2022.Common.Exceptions
{
    public class UnauthorizedActionException : Exception
    {
        protected string? Action { get; init; }

        public override string Message => $"You are not allowed to {Action}";
    }

    public class ChangePasswordException : UnauthorizedActionException
    {
        public ChangePasswordException()
        {
            Action = "Wrong old password";
        }
    }

    public class ModifyPostException : UnauthorizedActionException
    {
        public ModifyPostException()
        {
            Action = "modify this post";
        }
    }
    
    public class ModifyCommentException : UnauthorizedActionException
    {
        public ModifyCommentException()
        {
            Action = "modify this comment";
        }
    }

    public class DeleteStoryException : UnauthorizedActionException
    {
        public DeleteStoryException()
        {
            Action = "delete this story";
        }
    }

    
}
