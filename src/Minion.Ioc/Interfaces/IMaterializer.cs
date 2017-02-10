using System;
using System.Collections.Generic;
using Minion.Ioc.Models;
using Minion.Ioc.Profiler;

namespace Minion.Ioc.Interfaces
{
    public interface IMaterializer
    {
        dynamic MaterializeObject(Container container,
            Profile profile);

        dynamic MaterializeType(Container container,
            Profile profile,
            List<dynamic> parameters);
    }
}