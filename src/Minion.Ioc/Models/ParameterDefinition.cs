using System;

namespace Minion.Ioc.Models
{
    public class ParameterDefinition
    {
        public string Name { get; set; }
        public Type ContractType { get; set; }
        public bool HasDefaultConstructor { get; set; }
        public bool IsInterface { get; set; }
    }
}