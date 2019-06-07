using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Minion.CP.Abstraction.Types
{
	public enum MCode
	{
		None,
		M00,
		M01,
		M02,
		M03,
		M04,
		M05,
		M06,
		M07,
		M08,
		M09,
		M10,
		M11,
		M13,
		M19,
		M21,
		M22,
		M23,
		M24,
		M30,
		M41,
		M42,
		M43,
		M44,
		M48,
		M49,
		M52,
		M60,
		M98,
		M99
	}

	public static class MCodeExtensions
	{
		public static Regex _linePattern = new Regex(@"^ *([gGmM]{1}) *(\d+)(?: *([a-zA-Z]{1}) *(\d*))* *$");

		
	}
}
