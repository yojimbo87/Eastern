using System;

namespace Eastern
{
    public class OException : Exception
    {
        public OExceptionType Type { get; set; }

        public OException()
        {
        }

        public OException(OExceptionType exceptionType, string message) : base(message)
        {
            Type = exceptionType;
        }

        public OException(OExceptionType exceptionType, string message, Exception inner) : base(message, inner)
        {
            Type = exceptionType;
        }
    }
}
