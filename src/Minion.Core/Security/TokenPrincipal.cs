using System;
using System.Collections.Generic;
using System.Security.Principal;
using Newtonsoft.Json;

namespace Minion.Security
{
	public class TokenPrincipal : IUser
	{
		public string Email { get; set; }
		public bool IsActivated { get; set; }
		public bool IsExternalDepartment { get; set; }
		public bool IsService { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string TokenId { get; set; }
		public string Unit { get; set; }
		public string User { get; set; }
		public int UserId { get; set; }
		public string UserSID { get; set; }

		public IIdentity Identity { get; private set; }
		public string UserPrincipalName { get; set; }

		[JsonIgnore]
		public Dictionary<Guid, Permission> Permissions { get; set; }

		public TokenPrincipal()
		{
			TokenIdentity tokenIdentity = new TokenIdentity()
			{
				AuthenticationType = "ActiveDirectory",
				IsAuthenticated = false,
				Name = "Anonymous"
			};
			Identity = tokenIdentity;
		}

		public void Authenticate(string userPrincipalName)
		{
			TokenIdentity tokenIdentity = new TokenIdentity()
			{
				IsAuthenticated = true,
				Name = userPrincipalName
			};
			Identity = tokenIdentity;
			UserPrincipalName = userPrincipalName;
		}

		public bool IsInRole(string role)
		{
			throw new NotImplementedException();
		}
	}
}