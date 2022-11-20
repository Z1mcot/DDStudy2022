using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DDStudy2022.Common.Exceptions
{
    public class SessionTimeoutException : Exception
    {
        public override string Message => $"session timeout";
    }

    public class WrongPasswordException : Exception
    {
        public override string Message => $"wrong password";
    }

    public class AccountDeactivatedException : Exception
    {
        public override string Message => $"account is suspended";
    }
}
