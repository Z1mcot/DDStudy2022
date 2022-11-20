using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.Common.Exceptions
{
    public class ClaimConversionException : Exception
    {
        public string? Model { get; set; }

        public override string Message => $"{Model} claim is invalid";
    }

    public class IdClaimConversionException : ClaimConversionException
    {
        public IdClaimConversionException()
        {
            Model = "id";
        }
    }

    public class RefreshTokenClaimConversionException : ClaimConversionException
    {
        public RefreshTokenClaimConversionException()
        {
            Model = "refreshToken";
        }
    }

    public class SessionIdClaimConversionException : ClaimConversionException
    {
        public SessionIdClaimConversionException()
        {
            Model = "sessionId";
        }
    }
}
