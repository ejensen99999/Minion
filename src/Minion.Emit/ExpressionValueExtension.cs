using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Minion.Emit
{
	public static class ExpressionExtensions
	{
		public static object ExtractValue(this Expression expression)
		{
			var nodeType = expression.NodeType;
			var output = new object();

			switch (nodeType)
			{
				case ExpressionType.Constant:
					output = ExtractValue(expression as ConstantExpression);
					break;

				case ExpressionType.MemberAccess:
					output = ExtractValue(expression as MemberExpression);
					break;

				case ExpressionType.NewArrayInit:
					output = ExtractValue(expression as NewArrayExpression);
					break;

				case ExpressionType.New:
					var newExpression = expression as NewExpression;
					var parameters = ExtractNewValues(newExpression).ToArray();
					output = newExpression.Constructor.Invoke(parameters);
					break;

				case ExpressionType.UnaryPlus:
					output = ExtractValue(expression as UnaryExpression);
					break;

				case ExpressionType.Parameter:
				default:
					break;
			}

			return output;
		}

		private static IList CreateList(Type type)
		{
			return (IList)typeof(List<>).MakeGenericType(type).GetConstructor(new Type[0]).Invoke(null, null);
		}

		private static IEnumerable<object> ExtractNewValues(this NewExpression newExpression)
		{
			foreach (var argumentExpression in newExpression.Arguments)
			{
				yield return ExtractValue(argumentExpression);
			}
		}

		private static object ExtractValue(ConstantExpression constantExpression)
		{
			var output = new object();

			if (constantExpression.Value is Expression)
			{
				output = ExtractValue((Expression)constantExpression.Value);
			}
			else
			{
				if (constantExpression.Type == typeof(string) ||
				    constantExpression.Type.IsPrimitive ||
				    constantExpression.Type.IsEnum ||
				    constantExpression.Value == null)
					output = constantExpression.Value;
			}

			return output;
		}

		private static object ExtractValue(MemberExpression memberExpression)
		{
			var output = new object();
			var constExpression = (ConstantExpression)memberExpression.Expression;
			var valIsConstant = constExpression != null;
			var declaringType = memberExpression.Member.DeclaringType;
			object declaringObject = memberExpression.Member.DeclaringType;

			if (valIsConstant)
			{
				declaringType = constExpression.Type;
				declaringObject = constExpression.Value;
			}

			var member = declaringType.GetMember(memberExpression.Member.Name, MemberTypes.Field | MemberTypes.Property, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Single();

			if (member.MemberType == MemberTypes.Field)
			{
				output = ((FieldInfo)member).GetValue(declaringObject);
			}
			else
			{
				output = ((PropertyInfo)member).GetGetMethod(true).Invoke(declaringObject, null);
			}

			return output;
		}

		private static object ExtractValue(NewArrayExpression newArrayExpression)
		{
			var type = newArrayExpression.Type.GetElementType();

			if (type is IConvertible)
			{
				return ExtractConvertibleTypeArrayConstants(newArrayExpression, type);
			}

			return ExtractNonConvertibleArrayConstants(newArrayExpression, type);
		}

		private static object ExtractValue(UnaryExpression unaryExpression)
		{
			return ExtractValue(unaryExpression.Operand);
		}

		private static IEnumerable<object> ExtractConvertibleTypeArrayConstants(NewArrayExpression newArrayExpression, Type type)
		{
			var arrayElements = CreateList(type);

			foreach (var arrayElementExpression in newArrayExpression.Expressions)
			{
				var arrayElement = ((ConstantExpression)arrayElementExpression).Value;
				arrayElements.Add(Convert.ChangeType(arrayElement, arrayElementExpression.Type, null));
			}

			yield return ToArray(arrayElements);
		}

		private static object ExtractNonConvertibleArrayConstants(NewArrayExpression newArrayExpression, Type type)
		{
			var arrayElements = CreateList(type);

			foreach (var arrayElementExpression in newArrayExpression.Expressions)
			{
				object arrayElement;

				if (arrayElementExpression is ConstantExpression)
				{
					arrayElement = ((ConstantExpression)arrayElementExpression).Value;
				}
				else
				{
					arrayElement = ExtractValue(arrayElementExpression);
				}

				if (arrayElement is object[])
				{
					foreach (var item in (object[])arrayElement)
					{
						arrayElements.Add(item);
					}
				}
				else
				{
					arrayElements.Add(arrayElement);
				}
			}

			return ToArray(arrayElements);
		}

		private static IEnumerable<object> ToArray(IList list)
		{
			var toArrayMethod = list.GetType().GetMethod("ToArray");
			yield return toArrayMethod.Invoke(list, new Type[] { });
		}
	}
}
