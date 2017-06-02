using System;

namespace Minion.Injector.Exceptions
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
