using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Minion.Ioc.Interfaces;
using Minion.Ioc.Models;

[assembly:InternalsVisibleTo("Minion.Tests")]

namespace Minion.Ioc.Profiler
{
    public class ParameterProfile : IParameterProfile
    {
        public ConstructorInfo Ctor { get; set; }

        public List<ParameterDefinition> Parameters { get; }

        public int Magnitude { get; }

        public bool IsValid { get; private set; } = true;

        public ParameterProfile(ConstructorInfo ctor)
        {
            Ctor = ctor;
            Parameters = GetConstructorSignature(ctor);
            Magnitude = Parameters.Count;
        }

        internal List<ParameterDefinition> GetConstructorSignature(ConstructorInfo ctorInfo)
        {
            var output = new List<ParameterDefinition>();

            var parametersInfo = ctorInfo.GetParameters();

            foreach (var parameter in parametersInfo)
            {
                TypeInfo paramInfo;
                if (IsAcceptableParameter(parameter.ParameterType, out paramInfo))
                {
                    var iocParam = new ParameterDefinition
                    {
                        Name = parameter.Name,
                        ContractType = parameter.ParameterType,
                        HasDefaultConstructor = HasDefaultConstructor(paramInfo),
                        IsInterface = paramInfo.IsInterface
                    };

                    output.Add(iocParam);
                }
                else
                {
                    IsValid = false;
                    break;
                }
            }

            return output;
        }

        internal static bool IsAcceptableParameter(Type parameter,
            out TypeInfo paramInfo)
        {
            paramInfo = parameter.GetTypeInfo();

            var output = paramInfo.IsInterface
                         || (paramInfo.IsClass
                             && paramInfo.IsPublic);

            return output;
        }

        internal static bool HasDefaultConstructor(TypeInfo info)
        {
            var output = info.GetConstructors().Length >= 1 
                && info.GetConstructors()[0].GetParameters().Length == 0;

            return output;
        }
    }
}
