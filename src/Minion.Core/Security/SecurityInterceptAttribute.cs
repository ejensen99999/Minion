using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Minion.Security
{
	public class SecurityInterceptAttribute : ActionFilterAttribute
	{
		private const string _accessDenied = "No access allowed - insufficient permissions";
		private const string _tokenMismatch = "Authentication token passed via header does not match expected token - access denied";

		private ILogger<ICoreConfiguration> _log;
		private ICoreConfiguration _config;
		private ISecurityAssets _sa;
		private string _assemblyName = "";

	    public string Description { get; set; }

	    public Guid GroupId { get; set; }

	    public Guid Id { get; }

		public SecurityInterceptAttribute()
		{
		}

		public SecurityInterceptAttribute(string guidString) : this()
		{
			Id = ValidateId(guidString);
		}

		private void Initialize(FilterContext context)
		{
			if (_log == null)
			{
				_log = context.GetService<ILogger<ICoreConfiguration>>();
				_config = context.GetService<ICoreConfiguration>();
				_sa = context.GetService<ISecurityAssets>();

				var assembly = context is ActionExecutingContext ?
					((ActionExecutingContext)context).GetAssembly() :
					((ResultExecutedContext)context).GetAssembly();

				_assemblyName = assembly.FullName;
				_sa.Register(assembly);
			}
		}

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			Initialize(context);

			var interceptId = GroupId != Guid.Empty ? GroupId : Id;
			var isAuthorized = _sa.IsAuthorized(context.HttpContext, _assemblyName, interceptId);

			if (!isAuthorized)
			{
				_log.LogWarning("No access allowed - insufficient permissions");
				throw new Exception("No access allowed - insufficient permissions");
			}
		}

		public override void OnResultExecuted(ResultExecutedContext context)
		{
			//var t = context.Items();
		}


		private Guid ValidateId(string input)
		{
			Guid guid;
			if (!Guid.TryParse(input, out guid))
			{
				throw new ArgumentException("string could not be parsed into a Guid");
			}
			if (guid == Guid.Empty)
			{
				throw new ArgumentException("string entered cannot represent an empty Guid");
			}
			return guid;
		}
	}
}
