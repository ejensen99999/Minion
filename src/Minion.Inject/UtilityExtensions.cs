using Minion.Inject.Aspects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Minion.Inject
{
	public static class UtilityExtensions
	{

        public static bool InheritsFrom(this Type obj1,
		    Type obj2)
		{
			bool output;

			var info1 = obj1.GetTypeInfo();

			output = info1.ImplementedInterfaces.Contains(obj2);

			return output;
		}

		public static bool DoesNotContainKey<V>(this ConcurrentDictionary<string, V> dictionary,
			Type type)
		{
			var key = type.GetTypeName();
			return !dictionary.ContainsKey(key);
		}

		public static bool TryAdd<V>(this ConcurrentDictionary<string, V> dictionary,
		    Type type,
		    V obj)
		{
			var key = type.GetTypeName();
			return dictionary.TryAdd(key, obj);
		}

		public static V TryGet<V>(this ConcurrentDictionary<string, V> dictionary,
			Type type)
		{
			var key = type.GetTypeName();
			var obj = default(V);
			dictionary.TryGetValue(key, out obj);

			return obj;
		}

	    public static string GetTypeName(this Type type)
	    {
	        return type.FullName;
	    }
	}
}

