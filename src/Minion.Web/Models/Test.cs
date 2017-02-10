using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minion.Web.Models
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
