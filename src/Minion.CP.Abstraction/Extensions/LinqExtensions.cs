﻿using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minion.CP.Abstraction
{
	public static class LinqExtensions
	{
		public static IEnumerable<T> ForAll<T>(this IEnumerable<T> collection, Action<T> action)
		{
			foreach (var item in collection)
			{
				action(item);
				yield return item;
			}
		}
	}
}
