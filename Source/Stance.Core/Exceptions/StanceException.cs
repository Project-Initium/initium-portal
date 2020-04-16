// TOKEN_COPYRIGHT_TEXT

using System;

namespace Stance.Core.Exceptions
{
    public class StanceException : Exception
    {
        public StanceException()
        {
        }

        public StanceException(string message)
            : base(message)
        {
        }

        public StanceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}