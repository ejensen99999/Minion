using System;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.AspNetCore.Http;
using Minion.Security;

namespace Minion.ServiceModel
{
	public class ChannelBase<T> : ClientBase<T>, IChannelProxy<T>
	where T : class
	{
		public T Operations
		{
			get
			{
				return Channel;
			}
		}

		public ChannelBase()
		{
		}

		public ChannelBase(HttpContext context)
		{
			SetSecurityContext(context);
		}

		public ChannelBase(string endpointConfigurationName) : base(endpointConfigurationName)
		{
		}

		public ChannelBase(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public ChannelBase(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public ChannelBase(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		public ChannelBase(HttpContext context, string endpointConfigurationName) : base(endpointConfigurationName)
		{
			SetSecurityContext(context);
		}

		public ChannelBase(TokenPrincipal principal)
		{
			SetSecurityContext(principal);
		}

		public void Dispose()
		{
			Abort();
		}

		private TokenPrincipal DuplicatePrincipalForAuth(TokenPrincipal principal)
		{
			TokenPrincipal tokenPrincipal = new TokenPrincipal()
			{
				Name = principal.Name,
				TokenId = principal.TokenId,
				UserPrincipalName = principal.UserPrincipalName,
				UserSID = principal.UserSID,
				User = principal.User,
				Unit = principal.Unit,
				Phone = principal.Phone,
				IsService = principal.IsService,
				IsExternalDepartment = principal.IsExternalDepartment
			};
			TokenPrincipal tokenPrincipal1 = tokenPrincipal;
			tokenPrincipal1.Authenticate(principal.UserPrincipalName);
			return tokenPrincipal1;
		}

		public T SetHeaderContainer<V>(V container)
		{
			MessageHeader<V> messageHeader = new MessageHeader<V>(container);
			OperationContextScope operationContextScope = new OperationContextScope(base.InnerChannel);
			OperationContext.Current.OutgoingMessageHeaders.Add(messageHeader.GetUntypedHeader(typeof(V).ToString(), typeof(V).Namespace));
			return Channel;
		}

		public T SetSecurityContext(HttpContext context)
		{
			IPrincipal user = context.User;
			if (user == null ? false : user is TokenPrincipal)
			{
				SetHeaderContainer<TokenPrincipal>(DuplicatePrincipalForAuth((TokenPrincipal)user));
			}
			return Channel;
		}

		public T SetSecurityContext(TokenPrincipal principal)
		{
			if (principal != null)
			{
				SetHeaderContainer<TokenPrincipal>(principal);
			}
			return Channel;
		}
	}
}