using System;

namespace Minion.Inject.Exceptions
{
    public class InvalidConstructorException: Exception
    {
        public InvalidConstructorException(string message)
            : base(message)
        {
        }

        public InvalidConstructorException(string message,
            Exception exception)
            :base(message, exception)
        {
        }
    }
}
