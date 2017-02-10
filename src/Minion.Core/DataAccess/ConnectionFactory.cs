using System;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Minion.DataAccess
{
	public class ConnectionFactory
	{
		public static T ExecuteQuery<T>(ILogger<CoreConfiguration> log, ICoreConfiguration config, Func<SqlConnection, SqlCommand, T> function)
		{
			return ConnectionFactory.ExecuteQuery<T>(log, config.Connection.Data, function);
		}

		public static T ExecuteQuery<T>(ILogger<CoreConfiguration> log, string connectionString, Func<SqlConnection, SqlCommand, T> function)
		{
			var output = default(T);
            
			try
			{
				using (var sqlConnection = new SqlConnection(connectionString))
				using (var sqlCommand = sqlConnection.CreateCommand())
				{
					sqlConnection.Open();
					output = function(sqlConnection, sqlCommand);
				}
			}
			catch (Exception exception1)
			{
				var exception = exception1;
				var str = "ConnectionFactory caught internal exception";
				log.LogError(str, exception);
				throw new ConnectionFactoryException(str, exception);
			}
			return output;
		}

		public static void ExecuteQuery(ILogger<CoreConfiguration> log, ICoreConfiguration config, Action<SqlConnection, SqlCommand> action)
		{
			ConnectionFactory.ExecuteQuery(log, config.Connection.Data, action);
		}

		public static void ExecuteQuery(ILogger<CoreConfiguration> log, string connectionString, Action<SqlConnection, SqlCommand> action)
		{
			try
			{
				using (var sqlConnection = new SqlConnection(connectionString))
				using (var sqlCommand = sqlConnection.CreateCommand())
				{
					sqlConnection.Open();
					action(sqlConnection, sqlCommand);
				}
			}
			catch (Exception exception1)
			{
				var exception = exception1;
				var str = "ConnectionFactory caught internal exception";
				log.LogError(str, exception);
				throw new ConnectionFactoryException(str, exception);
			}
		}
	}
}