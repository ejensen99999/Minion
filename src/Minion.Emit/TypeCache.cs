using System;
using System.Collections.Concurrent;

namespace Minion.Emit
{
	public sealed class TypeCache: ITypeCache
	{
		//private static volatile TypeCache _instance;
		private static readonly object _syncTypes;
		private static readonly ConcurrentDictionary<string, Type> _typeCache;

		public TypeCache()
		{
		}

		static TypeCache()
		{
			_syncTypes = new Object();
			_typeCache = new ConcurrentDictionary<string, Type>();
		}

		public Type GetType<T>(Func<ScopeNamespace, string> scope, IEmitter emitter)
			where T : class
		{
			return GetType<T>(scope(new ScopeNamespace(typeof(T))), emitter);
		}

		public Type GetType<T>(string key, IEmitter emitter)
			where T : class
		{
			Type target;
			if (!_typeCache.TryGetValue(key, out target))
			{
				lock (_syncTypes)
				{
					if (!_typeCache.TryGetValue(key, out target))
					{
						target = emitter.GenerateType(typeof(T));

						_typeCache.TryAdd(key, target);
					}
				}
			}
			return target;
		}
	}
}
