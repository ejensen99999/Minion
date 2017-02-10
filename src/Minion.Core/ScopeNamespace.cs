using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minion
{
	public class ScopeNamespace
	{
		public string _space;
		public string _name;

		private string NewName(string scope = "")
		{
			var myScope = string.IsNullOrWhiteSpace(scope) ? "" : scope + ".";
			var output = _space + "." + myScope + _name;
			return output;
		}

		public ScopeNamespace(Type baseType)
		{
			_space = baseType.Namespace;
			_name = baseType.Name;
		}

		public ScopeNamespace(string space, string name)
		{
			_space = space;
			_name = name;
		}

		public static string GetScopeName<T>(string scope = "")
		{
			return GetScopeName(typeof (T), scope);
		}

		public static string GetScopeName(Type baseType, string scope = "")
		{
			var space = baseType.Namespace;
			var name = baseType.Name;
			var myScope = string.IsNullOrWhiteSpace(scope) ? "" : scope + ".";
			var output = space + "." + myScope + name;
			return output;
		}

		public string Key
		{
			get { return NewName(); }
		}

		public string Aspect
		{
			get { return NewName("Aspect"); }
		}

		public string Cassandra
		{
			get { return NewName("Cassandra"); }
		}

		public string Constructor { get; set; }

		public string Data
		{
			get { return NewName("Data"); }
		}

		public string Difference
		{
			get { return NewName("Difference"); }
		}

		public string Setter
		{
			get { return NewName("Setter"); }
		}
	}
}