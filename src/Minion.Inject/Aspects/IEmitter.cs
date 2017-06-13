using System;
using System.Reflection;

namespace Minion.Inject.Aspects
{
     public interface IEmitter
     {
          Type CreateAspectProxyType<T>(ConstructorInfo ctor = null)
               where T : class;

          Type CreateAspectProxyType(Type baseType,
               ConstructorInfo ctor = null);
     }
}
