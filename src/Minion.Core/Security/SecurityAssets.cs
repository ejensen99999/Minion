using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Minion.DataControl;

namespace Minion.Security
{
	public class SecurityAssets : ISecurityAssets
	{
		private readonly ILogger<ICoreConfiguration> _log;
		private readonly ICoreConfiguration _config;

		private static object _synclock = new object();
		private static Registrations _registrations = new Registrations();

		public SecurityAssets(ILogger<ICoreConfiguration> log, ICoreConfiguration config)
		{
			_log = log;
			_config = config;
		}

		public IEnumerable<Asset> Spider(Assembly assembly)
		{
			var guids = new Dictionary<Guid, Asset>();

			try
			{
				var assemblyName = assembly.FullName;
				var types = assembly.GetTypes();

				for (int j = 0; j < types.Length; j++)
				{
					var type = types[j];
					var customAttribute = type
						.GetTypeInfo()
						.GetCustomAttribute<SecurityInterceptAttribute>();

					if (customAttribute != null)
					{
						var id = customAttribute.Id;
						var desc = customAttribute.Description;
						var name = type.Name;

						var asset = new Asset()
						{
							UniqueId = id,
							AssemblyName = assemblyName,
							ControllerId = id,
							ControllerName = name,
							Description = desc,
							Type = SecurityAssetType.Controller
						};

						if (asset.ControllerId != Guid.Empty)
						{
							guids.Add(asset.ControllerId, asset);
						}

						var methods = type.GetMethods();

						for (int k = 0; k < (int)methods.Length; k++)
						{
							var methodInfo = methods[k];
							var securityAttribute = methodInfo
								.GetCustomAttribute<SecurityInterceptAttribute>();

							if (securityAttribute != null)
							{
								var id1 = securityAttribute.Id;
								var desc1 = securityAttribute.Description;
								var name1 = methodInfo.Name;

								var asset4 = new Asset()
								{
									SuperiorId = id,
									UniqueId = id1,
									AssemblyName = assemblyName,
									ControllerId = id,
									ControllerName = name,
									ActionId = id1,
									ActionName = name1,
									Description = desc1,
									Type = SecurityAssetType.Action
								};

								var asset5 = asset4;

								if (asset5.ActionId != Guid.Empty)
								{
									guids.Add(asset5.ActionId, asset5);
								}
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				_log.LogError("Could not complete asset inventory", exception);
			}

			return guids.Values;
		}

		public bool IsAuthorized(HttpContext context, string key, Guid interceptId)
		{
			//var user = context.User.
			var output = _registrations[key].IsIndexed(interceptId);
			return output;
		}

		public void Register(Assembly assembly)
		{
			var key = assembly.FullName;

			if (!_registrations.IsIndexed(key))
			{
				lock (_synclock)
				{
					if (!_registrations.IsIndexed(key))
					{
						var data = Spider(assembly);

						foreach (var datum in data)
						{
							_registrations[key][datum.UniqueId] = datum;
						}
					}
				}
			}
		}

		private class Registrations : Indice<string, AssemblyNode>
		{
		}

		private class AssemblyNode : Indice<Guid, Asset>
		{
		}
	}
}