using System;

namespace Minion.Emit
{
	public interface ITypeCache
	{
		Type GetType<T>(Func<ScopeNamespace, string> scope, IEmitter emitter) 
			where T : class;

		Type GetType<T>(string key, IEmitter emitter) 
			where T : class;
	}
}
