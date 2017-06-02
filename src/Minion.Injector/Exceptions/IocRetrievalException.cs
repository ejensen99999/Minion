using System;

namespace Minion.Injector.Exceptions
{
    public class IocRetrievalException : Exception
    {
        public IocRetrievalException(string message)
            : base(message)
        {
        }

        public IocRetrievalException(string message,
            Exception exception)
            : base(message, exception)
        {
        }
    }
}
