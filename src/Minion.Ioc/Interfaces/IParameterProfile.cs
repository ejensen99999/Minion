using System.Collections.Generic;
using System.Reflection;
using Minion.Ioc.Models;
using Minion.Ioc.Profiler;

namespace Minion.Ioc.Interfaces
{
    public interface IParameterProfile
    {
        ConstructorInfo Ctor { get; set; }
        List<ParameterDefinition> Parameters { get; }
        int Magnitude { get; }
        bool IsValid { get; }
    }
}