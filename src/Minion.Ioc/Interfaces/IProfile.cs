using System;
using System.Collections.Generic;
using Minion.Ioc.Models;
using Minion.Ioc.Profiler;

namespace Minion.Ioc.Interfaces
{
    public interface IProfile
    {
        Type Concrete { get; }
        Type Contract { get; }
        Lifetime Lifecycle { get; }
        List<ParameterDefinition> Parameters { get; }
        dynamic Instance { get; set; }
    }
}