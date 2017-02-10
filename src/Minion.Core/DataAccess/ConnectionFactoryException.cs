using System;

namespace Minion.DataAccess
{
	public class ConnectionFactoryException : Exception
	{
		public ConnectionFactoryException()
		{
		}

		public ConnectionFactoryException(string message) : base(message)
		{
		}

		public ConnectionFactoryException(string message, Exception ex) : base(message, ex)
		{
		}
	}
}