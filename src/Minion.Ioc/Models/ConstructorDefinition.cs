using Minion.Ioc.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Minion.Ioc.Models
{
     public class ConstructorDefinition
     {
          public List<ParameterDefinition> Parameters { get; }
          public IConstructor Constructor { get; }

          public ConstructorDefinition(List<ParameterDefinition> parameters,
               IConstructor constructor)
          {
               Parameters = parameters;
               Constructor = constructor;
          }
     }
}