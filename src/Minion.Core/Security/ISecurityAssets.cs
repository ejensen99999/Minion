using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace Minion.Security
{
	public interface ISecurityAssets
	{
		IEnumerable<Asset> Spider(Assembly assembly);

		bool IsAuthorized(HttpContext context, string key, Guid interceptId);

		void Register(Assembly assembly);
	}
}
