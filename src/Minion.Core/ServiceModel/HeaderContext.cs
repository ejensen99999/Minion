using System.ServiceModel;

namespace Minion.ServiceModel
{
	public static class HeaderContext<T>
	{
		public static T GetHeader()
		{
			T header = default(T);
			OperationContext current = OperationContext.Current;
			if (current != null)
			{
				if (current.IncomingMessageHeaders.FindHeader(typeof(T).ToString(), typeof(T).Namespace) >= 0)
				{
					header = current.IncomingMessageHeaders.GetHeader<T>(typeof(T).ToString(), typeof(T).Namespace);
				}
			}
			return header;
		}

		public static void SetHeader(IContextChannel channel, T header)
		{
			MessageHeader<T> messageHeader = new MessageHeader<T>(header);
			OperationContextScope operationContextScope = new OperationContextScope(channel);
			OperationContext.Current.OutgoingMessageHeaders.Add(messageHeader.GetUntypedHeader(typeof(T).ToString(), typeof(T).Namespace));
		}
	}
}