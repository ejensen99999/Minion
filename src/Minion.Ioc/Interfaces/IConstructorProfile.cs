using System;
using System.Collections.Generic;
using Minion.Ioc.Models;
using Minion.Ioc.Profiler;

namespace Minion.Ioc.Interfaces
{
    public interface IConstructorProfile
    {
        ConstructorDefinition GetTargetConstructor(Type contract,
            Type concrete);
    }
}