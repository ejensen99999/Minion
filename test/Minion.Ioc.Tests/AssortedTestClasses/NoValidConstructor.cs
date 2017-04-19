using Minion.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minion.Tests.AssortedTestClasses
{
    public class NoValidConstructor
    {
        public NoValidConstructor(Lifetime life)
        {
        }
    }
}
