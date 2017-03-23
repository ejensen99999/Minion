using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Extensions.DependencyInjection;
using Minion.Ioc.Aspects;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;

namespace Minion.Ioc
{
    public static class Helper
    {
        public static bool CheckDependancyChain(Type input, Type searchType)
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

        public static AspectEventContext CreateAspectContext(object callerReference, string memberName = "")
        {
            return new AspectEventContext
            {
                CallerReference = callerReference,
                CallerType = callerReference.GetType(),
                MemberName = memberName
            };
        }

        public static void CreateConstructorNameSpace(this ScopeNamespace name, object[] args)
        {
            var output = "";

            foreach (var i in args)
            {
                var typeDesc = "";
                if (i == null)
                {
                    typeDesc = "-NULL";
                }
                else
                {
                    typeDesc = "-" + i.GetType().ToString();
                }

                output += typeDesc;
            }

            name.Constructor = name.Aspect + output;
        }

        public static TContract Get<TContract>(this IServiceProvider provider)
        {
            var output = provider.GetService(typeof(TContract));

            return (TContract)output;
        }

        public static IEnumerable<CustomAttributeContainer> GetAttributeContainers(IEnumerable<Attribute> attributes, IList<CustomAttributeData> attributesData)
        {
            var output = new List<CustomAttributeContainer>();
            var atts = attributes.ToArray();
            var data = attributesData.ToArray();

            for (var i = 0; i < atts.Length; i++)
            {
                var attribute = atts[i];
                var datum = data[i];

                if (!CheckDependancyChain(attribute.GetType(), typeof(BaseAspect)))
                {
                    continue;
                }

                var baseIt = (BaseAspect)attribute;

                output.Add(new CustomAttributeContainer { Attribute = attribute, AttributeType = attribute.ToString(), Data = datum, Index = baseIt.Order });
            }

            var e = output.OrderBy(x => x.Index).ToArray();

            return e;
        }

        public static IList<CustomAttributeData> GetCustomAttributeData(this PropertyInfo prop)
        {
            return CustomAttributeData.GetCustomAttributes(prop);
        }

        public static IList<CustomAttributeData> GetCustomAttributeData(this MethodInfo method)
        {
            return CustomAttributeData.GetCustomAttributes(method);
        }

        public static bool InheritsFrom(this Type obj1, Type obj2)
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

        public static void LoadForValueType(this ILGenerator emitter, object value)
        {
            var type = value != null ? value.GetType().ToString() : "NULL";

            switch (type)
            {
                case "System.Boolean":
                    emitter.Emit((bool)value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                    break;

                case "System.Byte":
                    emitter.Emit(OpCodes.Ldstr, (Byte)value);
                    break;

                case "System.Char":
                    emitter.Emit(OpCodes.Ldstr, (Char)value); //???????
                    break;

                case "System.String":
                    emitter.Emit(OpCodes.Ldstr, (string)value);
                    break;

                case "System.Single":
                    emitter.Emit(OpCodes.Ldc_R4, (float)value);
                    break;

                case "System.Double":
                    emitter.Emit(OpCodes.Ldstr, (Double)value);
                    break;

                case "System.Int16":
                    emitter.Emit(OpCodes.Ldstr, (short)value);
                    break;

                case "System.Int32":
                    emitter.Emit(OpCodes.Ldc_I4_S, (int)value);
                    break;

                case "System.Int64":
                    emitter.Emit(OpCodes.Ldc_I8, (long)value);
                    break;

                case "System.Type":
                    emitter.Emit(OpCodes.Ldstr, (Type)value);
                    break;

                case "NULL":
                    emitter.Emit(OpCodes.Ldnull);
                    break;
            }

        }

        public static CustomParameterInfo ToCustomParameter(this ParameterInfo pi)
        {
            return new CustomParameterInfo
            {
                Attributes = pi.Attributes,
                Member = pi.Member,
                ParameterType = pi.ParameterType
            };
        }

        public static CustomParameterInfo[] ToCustomParameters(this ParameterInfo[] pis)
        {
            return pis.Select(x => x.ToCustomParameter()).ToArray();
        }

        public static Lifetime ToLifetime(this ServiceLifetime life)
        {
            var output = Lifetime.Transient;

            switch (life)
            {
                case ServiceLifetime.Scoped:
                    output = Lifetime.ThreadAsync;
                    break;
                case ServiceLifetime.Singleton:
                    output = Lifetime.Singleton;
                    break;
                case ServiceLifetime.Transient:
                default:
                    output = Lifetime.Transient;
                    break;
            }

            return output;
        }

        public static ServiceLifetime ToServiceLifetime(this Lifetime life)
        {
            var output = ServiceLifetime.Transient;

            switch (life)
            {
                case Lifetime.Scoped:
                    output = ServiceLifetime.Scoped;
                    break;
                case Lifetime.Singleton:
                    output = ServiceLifetime.Singleton;
                    break;
                case Lifetime.ThreadAsync:
                case Lifetime.Transient:
                default:
                    output = ServiceLifetime.Transient;
                    break;
            }

            return output;
        }
    }
}

