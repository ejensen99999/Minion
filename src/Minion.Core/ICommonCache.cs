using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Minion
{
	public interface ICommonCache
	{
		IMemoryCache Base { get; }
		void Add(string key, object value, bool sliding = false);
		void Add(string key, object value, TimeSpan lifeTime, bool sliding = false);
		void Add(string key, object value, MemoryCacheEntryOptions options);
		T Get<T>(string key);
		void Remove(string key);
		T TryGet<T>(string key, Func<T> retrieveFunction, bool sliding = false);
		T TryGet<T>(string key, Func<T> retrieveFunction, TimeSpan lifeTime, bool sliding = false);

	}
}
