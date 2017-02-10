using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace Minion.Security
{
	public interface IUser : IPrincipal
	{
		string Email
		{
			get;
			set;
		}

		bool IsActivated
		{
			get;
			set;
		}

		bool IsExternalDepartment
		{
			get;
			set;
		}

		bool IsService
		{
			get;
			set;
		}

		string Name
		{
			get;
			set;
		}

		Dictionary<Guid, Permission> Permissions
		{
			get;
			set;
		}

		string Phone
		{
			get;
			set;
		}

		string TokenId
		{
			get;
			set;
		}

		string Unit
		{
			get;
			set;
		}

		string User
		{
			get;
			set;
		}

		int UserId
		{
			get;
			set;
		}

		string UserPrincipalName
		{
			get;
		}

		string UserSID
		{
			get;
			set;
		}

		void Authenticate(string userPrincipalName);

	}
}