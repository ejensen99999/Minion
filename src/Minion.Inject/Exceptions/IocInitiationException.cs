using System;

namespace Minion.Inject.Exceptions
{
    public class IocInitiationException : Exception
    {
        public IocInitiationException(string message)
            : base(message)
        {
        }

        public IocInitiationException(string message,
            Exception exception)
            : base(message, exception)
        {
        }
    }
}