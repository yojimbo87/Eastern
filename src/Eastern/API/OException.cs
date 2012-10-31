using System;

namespace Eastern
{
    public class OException : Exception
    {
        public OExceptionType Type { get; set; }
        public string Description { get; set; }

        public OException(OExceptionType exceptionType, string exceptionString)
        {
            Type = exceptionType;
            Description = exceptionString;
        }
    }
}
