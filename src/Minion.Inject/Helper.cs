using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Minion.Inject
{
	public static class Helper
	{
		public static bool CheckDependancyChain(Type input,
		    Type searchType)
		{
			var output = false;
			var baseType = input.GetTypeInfo()
				.BaseType;

			if (baseType != searchType && baseType != null)
			{
				output = CheckDependancyChain(baseType, searchType);
			}
			else if (baseType == searchType)
			{
				output = true;
			}

			return output;
		}

		public static TContract Get<TContract>(this IServiceProvider provider)
		{
			var output = provider.GetService(typeof(TContract));

			return (TContract)output;
		}

		public static bool InheritsFrom(this Type obj1,
		    Type obj2)
		{
			bool output;

			var info1 = obj1.GetTypeInfo();

			output = info1.ImplementedInterfaces.Contains(obj2);

			return output;
		}

		public static bool IsInterface(this Type obj)
		{
			var info = obj.GetTypeInfo();

			return info.IsInterface;
		}

		public static bool IsValueType(this Type obj)
		{
			var info = obj.GetTypeInfo();

			return info.IsValueType;
		}

		public static bool ContainsKey<V>(this ConcurrentDictionary<string, V> dictionary,
			Type type)
		{
			var key = type.GetTypeName();
			return dictionary.ContainsKey(key);
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

		public static bool TryGetValue<V>(this ConcurrentDictionary<string, V> dictionary,
		  Type type,
		  out V obj)
		{
			var key = type.GetTypeName();
			return dictionary.TryGetValue(key, out obj);
		}

		public static string GetTypeName(this Type type)
		{
			return type.FullName;
		}
	}
}

