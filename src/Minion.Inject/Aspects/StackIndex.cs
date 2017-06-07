using System.Reflection.Emit;

namespace Minion.Inject.Aspects
{
     public class StackIndex
     {
          public LocalBuilder Output { get; set; }
          public LocalBuilder Store { get; set; }
          public LocalBuilder Bool { get; set; }
          public LocalBuilder Context { get; set; }
     }
}