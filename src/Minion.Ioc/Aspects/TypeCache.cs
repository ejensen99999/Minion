using Minion.Ioc.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Minion.Ioc.Aspects
{
     public interface ITypeCache
     {
          Type GetType<T>(ConstructorInfo ctor)
               where T : class;

          Type GetType(Type concrete,
               ConstructorInfo ctor);
     }

     public sealed class TypeCache: ITypeCache
	{
		private readonly object _syncTypes;
	     private readonly IEmitter _emitter;
		private readonly ConcurrentDictionary<string, Type> _typeCache;

		public TypeCache(IEmitter emitter)
		{
               _syncTypes = new object();
		     _emitter = emitter;
			_typeCache = new ConcurrentDictionary<string, Type>();
		}

	     public Type GetType<T>(ConstructorInfo ctor)
			where T : class
	     {
	          return GetType(typeof(T), ctor);
	     }

          public Type GetType(Type concrete,
	          ConstructorInfo ctor)
	     {
	          if(concrete != null)
               {
	               var key = concrete.FullName;
	               Type target;
	               if (!_typeCache.TryGetValue(key, out target))
	               {
	                    lock (_syncTypes)
	                    {
	                         if (!_typeCache.TryGetValue(key, out target))
	                         {
                                   target = _emitter.GenerateType(concrete);

	                              _typeCache.TryAdd(key, target);
	                         }
	                    }
	               }
                    concrete = target;
	          }

               return concrete;
	     }
	}
}
