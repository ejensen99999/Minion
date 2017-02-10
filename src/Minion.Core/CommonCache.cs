using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Minion
{
	public class CommonCache: ICommonCache
	{
		private readonly object _syncLock;
		private readonly IMemoryCache _cache;
		private readonly ILogger<CommonCache> _log;
		private readonly ICoreConfiguration _config;

		public IMemoryCache Base
		{
			get
			{
				return _cache;
			}
		}

		public CommonCache(ILogger<CommonCache> log, ICoreConfiguration config, IMemoryCache cache )
		{
			_syncLock = new object();
			_log = log;
			_config = config;
			_cache = cache;
		}

		public void Add(string key, object value, bool sliding = false)
		{
			Add(key, value, TimeSpan.FromMinutes(5), sliding);
		}

		public void Add(string key, object value, TimeSpan lifeTime, bool sliding = false)
		{
			var options = new MemoryCacheEntryOptions();

			if (!sliding)
			{
				options.AbsoluteExpiration = DateTimeOffset.Now.Add(lifeTime);
			}
			else
			{
				options.SlidingExpiration = lifeTime;
			}
			_cache.Set(key, value, options);
		}

		public void Add(string key, object value, MemoryCacheEntryOptions options)
		{
			_cache.Set(key, value, options);
		}

		public T Get<T>(string key)
		{
			return (T) _cache.Get(key);
		}

		public void Remove(string key)
		{
			_cache.Remove(key);
		}

		public T TryGet<T>(string key, Func<T> retrieveFunction, bool sliding = false)
		{
			var t = TryGet<T>(key, retrieveFunction, _config.Cache.Duration, sliding);
			return t;
		}

		public T TryGet<T>(string key, Func<T> retrieveFunction, TimeSpan lifeTime, bool sliding = false)
		{
			var t = default(T);
			try
            { 
				if (!_cache.TryGetValue(key, out t))
				{
					lock (_syncLock)
					{
						if (!_cache.TryGetValue(key, out t))
						{
							try
							{
								t = retrieveFunction();
							}
							catch (Exception exception)
							{
								throw new Exception("retrieve function failed", exception);
							}

							Add(key, t, sliding);
						}
					}
				}
			}
			catch (Exception exception1)
			{
				_log.LogError(exception1.ToString());
			}
			return t;
		}
	}
}