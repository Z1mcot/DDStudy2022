namespace DDStudy2022.Common.Exceptions;

public class NoPermissionException : Exception
{
    protected string? Action { get; init; }

    public override string Message => $"You are not allowed to {Action}";
}

public class PrivateAccountException : NoPermissionException
{
    public PrivateAccountException()
    {
        Action = "see this users posts.";
    }
}

public class PrivateAccountInfoException : NoPermissionException
{
    public PrivateAccountInfoException()
    {
        Action = "see this user info";
    }
}

public class PrivateAccountSubscribersException : NoPermissionException
{
    public PrivateAccountSubscribersException()
    {
        Action = "see this users subscribers";
    }
}

public class PrivateAccountSubscriptionException : NoPermissionException
{
    public PrivateAccountSubscriptionException()
    {
        Action = "see this subscription";
    }
}