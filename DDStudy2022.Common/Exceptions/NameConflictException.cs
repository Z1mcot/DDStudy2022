using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDStudy2022.Common.Exceptions
{
    public class NameConflictException : Exception
    {
        public string? Model { get; set; }

        public override string Message => $"{Model} already exists";
    }

    public class EmailConflictException : NameConflictException
    {
        public EmailConflictException()
        {
            Model = "account with this email";
        }
    }

    public class TempFileAlreadyExists : NameConflictException
    {
        public TempFileAlreadyExists()
        {
            Model = "temp file with this name";
        }
    }
}
