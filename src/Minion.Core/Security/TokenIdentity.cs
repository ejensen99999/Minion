using System.Security.Principal;

namespace Minion.Security
{
	public class TokenIdentity : IIdentity
	{
		public string AuthenticationType { get; set; }

		public bool IsAuthenticated { get; set; }

		public string Name { get; set; }
	}
}