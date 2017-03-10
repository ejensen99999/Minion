using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Minion.Ioc.Interfaces
{
     public interface IEmitter
     {
          Type GenerateType<T>(ConstructorInfo ctor = null)
               where T : class;

          Type GenerateType(Type baseType,
               ConstructorInfo ctor = null);
     }
}
