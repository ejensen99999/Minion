using Minion.Ioc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Minion.Ioc.Profiler
{
     public class PassThroughEmitter : IEmitter
     {
          public Type GenerateType(Type baseType, ConstructorInfo ctor = null)
          {
               return baseType;
          }

          public Type GenerateType<T>(ConstructorInfo ctor = null) where T : class
          {
               return GenerateType(typeof(T));
          }
     }
}
