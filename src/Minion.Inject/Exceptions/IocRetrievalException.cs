using System;

namespace Minion.Inject.Exceptions
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
