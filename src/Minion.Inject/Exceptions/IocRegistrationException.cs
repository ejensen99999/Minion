using System;

namespace Minion.Inject.Exceptions
{
    public class IocRegistrationException: Exception
    {
        public IocRegistrationException(string message)
            : base(message)
        {
        }

        public IocRegistrationException(string message,
            Exception exception)
            :base(message, exception)
        {
        }
    }
}
