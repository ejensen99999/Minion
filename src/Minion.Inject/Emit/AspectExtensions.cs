using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Minion.Inject.Aspects;

namespace Minion.Inject.Emit
{
    public static class AspectExtensions
    {
        //Do Not Remove!!! Necessary for the aspect to call after it's generated!!
        public static AspectEventContext CreateAspectContext(object callerReference,
            string memberName = "")
        {
            return new AspectEventContext
            {
                CallerReference = callerReference,
                CallerType = callerReference.GetType(),
                MemberName = memberName
            };
        }

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

        public static IEnumerable<CustomAttributeContainer> GetAttributeContainers(object[] attributes,
            IList<CustomAttributeData> attributesData)
        {
            var output = new List<CustomAttributeContainer>();
            var atts = attributes.ToArray();
            var data = attributesData.ToArray();

            for (var i = 0; i < atts.Length; i++)
            {
                var attribute = (Attribute)atts[i];
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

        public static void LoadForValueType(this ILGenerator emitter,
            object value)
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
    }
}
