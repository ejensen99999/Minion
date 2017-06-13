using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;

namespace Minion.Core.ServiceModel
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
                    output = (expression as ConstantExpression).Value;
                    break;

                case ExpressionType.MemberAccess:
                    output = getMemberAccessValue(expression);
                    break;

                case ExpressionType.NewArrayInit:
                    output = ExtractValue(expression as NewArrayExpression);
                    break;

                //case ExpressionType.New:
                //    var newExpression = expression as NewExpression;
                //    var parameters = ExtractNewValues(newExpression).ToArray();
                //    output = newExpression.Constructor.Invoke(parameters);
                //    break;

                case ExpressionType.UnaryPlus:
                    output = ExtractValue(expression as UnaryExpression);
                    break;

                case ExpressionType.Parameter:
                default:
                    break;
            }

            return output;
        }

        private static object getMemberAccessValue(Expression expression)
        {
            var memberInfo = ((dynamic)expression).Member as MemberInfo;
            var exp = ((dynamic)expression).Expression;
            var constant = exp as ConstantExpression;
            var value = constant.Value;
            var output = value.GetType()
                .GetField(memberInfo.Name)
                .GetValue(value);
            return output;
        }

        public static string GetQueryString(this object obj)
        {
            var properties = obj.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(x => x.Name + "=" + WebUtility.UrlEncode(x.GetValue(obj, null).ToString()));

            return string.Join("&", properties.ToArray());
        }
    }
}
