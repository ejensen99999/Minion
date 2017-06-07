using System;
using System.Reflection;

namespace Minion.Inject.Aspects
{
     public class CustomParameterInfo
     {
          public ParameterAttributes Attributes { get; set; }
          public MemberInfo Member { get; set; }
          public Type ParameterType { get; set; }
     }
}