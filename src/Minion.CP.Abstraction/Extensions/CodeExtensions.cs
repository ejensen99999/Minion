using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Minion.CP.Abstraction.Extensions
{
	public static class CodeExtensions
	{
		public static Regex _linePattern = new Regex(@"^ *([gGmM]{1} *\d+)(?: *([a-zA-Z]{1} *\d*))* *$", RegexOptions.Compiled);

		public static Code ParseCode(this string code)
		{
			var stuff = _linePattern.Matches(code);



			return null;
		}
	}

	public class Code
	{
		public List<Parameter> Paramters { get; set; }
	}

	public class MCode : Code
	{

	}

	public class GCode : Code
	{

	}

	public class Parameter
	{


	}
}
