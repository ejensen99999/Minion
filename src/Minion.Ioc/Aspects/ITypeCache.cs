using System;

namespace Minion.Ioc.Aspects
{
	public interface ITypeCache
	{
		Type GetType<T>(Func<ScopeNamespace, string> scope, IEmitter emitter) 
			where T : class;

		Type GetType<T>(string key, IEmitter emitter) 
			where T : class;
	}
}
