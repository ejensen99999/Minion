namespace Minion
{
	public static class ComparisonExtensions
	{
		public static bool IsGreaterThanOrEqualTo(this string input, string second)
		{
			return string.Compare(input, second) >= 0;
		}

		public static bool IsGreaterThan(this string input, string second)
		{
			return string.Compare(input, second) > 0;
		}

		public static bool IsLessTanOrEqualTo(this string input, string second)
		{
			return string.Compare(input, second) <= 0;
		}

		public static bool IsLessThan(this string input, string second)
		{
			return string.Compare(input, second) < 0;
		}
	}
}