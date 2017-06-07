using System;
using System.Reflection;

namespace Minion.Inject.Aspects
{
     public interface IEmitter
     {
          Type GenerateType<T>(ConstructorInfo ctor = null)
               where T : class;

          Type GenerateType(Type baseType,
               ConstructorInfo ctor = null);
     }
}
