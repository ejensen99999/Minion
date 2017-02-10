using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Minion
{
	public static class Extensions
	{
		public static string ToFieldIdentifier(this object obj)
		{
			return nameof(obj);
		}

		public static string ToFieldIdentifier(this LambdaExpression expression)
		{
			return expression.Body.ToFieldIdentifier();
		}

		public static string ToFieldIdentifier(this Expression expression)
		{
			var output = "";
			switch (expression.NodeType)
			{
				case ExpressionType.Call:
					var method = expression as MethodCallExpression;
					var args = new List<string>();

					foreach (var i in method.Arguments)
						args.Add(ToFieldIdentifier(i));

					output = String.Join("|", args);
					break;

				case ExpressionType.Constant:
					output = (expression as ConstantExpression).Value.ToString();
					break;

				case ExpressionType.Convert:
					output = (expression as UnaryExpression).Operand.ToFieldIdentifier();
					break;

				case ExpressionType.MemberAccess:
					output = ((expression as MemberExpression).Member as PropertyInfo).Name;
					break;

				case ExpressionType.Quote:
					output = ((expression as UnaryExpression).Operand as LambdaExpression).ToFieldIdentifier();
					break;

				default:
					throw new InvalidOperationException("The expression you passed in is of type: \"" + expression.NodeType.ToString() + "\" and is not supported");
			}

			return output;
		}

		public static Type ToFieldType(this LambdaExpression expression)
		{
			return expression.Body.ToFieldType();
		}

		public static Type ToFieldType(this Expression expression)
		{
			Type type = null;
			ExpressionType nodeType = expression.NodeType;
			switch (nodeType)
			{
				case ExpressionType.Call:
					{
						MethodCallExpression methodCallExpression = expression as MethodCallExpression;
						List<string> strs = new List<string>();
						foreach (Expression argument in methodCallExpression.Arguments)
						{
							strs.Add(argument.ToFieldIdentifier());
						}
						type = typeof(MethodInfo);
						break;
					}
				case ExpressionType.Coalesce:
				case ExpressionType.Conditional:
					{
						throw new InvalidOperationException(string.Concat("The expression you passed in is of type: \"",
							expression.NodeType.ToString(), "\" and is not supported"));
					}
				case ExpressionType.Constant:
					{
						type = (expression as ConstantExpression).Value.GetType();
						break;
					}
				case ExpressionType.Convert:
					{
						type = (expression as UnaryExpression).Operand.ToFieldType();
						break;
					}
				default:
					{
						if (nodeType == ExpressionType.MemberAccess)
						{
							type = ((expression as MemberExpression).Member as PropertyInfo).PropertyType;
							break;
						}
						else if (nodeType == ExpressionType.Quote)
						{
							type = ((expression as UnaryExpression).Operand as LambdaExpression).ToFieldType();
							break;
						}
						else
						{
							throw new InvalidOperationException(string.Concat("The expression you passed in is of type: \"",
								expression.NodeType.ToString(), "\" and is not supported"));
						}
					}
			}
			return type;
		}

	    public static bool IsNumeric<T>(this T input)
	    {
            var valueType = typeof(T);

            bool isAtomic =
                valueType == typeof(bool) ||
                valueType == typeof(char) ||
                valueType == typeof(byte) ||
                valueType == typeof(sbyte) ||
                valueType == typeof(short) ||
                valueType == typeof(ushort) ||
                valueType == typeof(int) ||
                valueType == typeof(uint) ||
                valueType == typeof(float);

            if (!isAtomic && IntPtr.Size == 8)
            {
                isAtomic |= valueType == typeof(double) || valueType == typeof(long);
            }

            return isAtomic;
	    }
    }
}
