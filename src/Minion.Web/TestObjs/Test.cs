using System;

namespace Minion.Web.TestObjs
{
    public interface ITest
    {
        Guid Id { get; set; }
    }

    public class Test: ITest
    {
        public Guid Id { get; set; }
    }
}
