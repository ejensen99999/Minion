using Minion.DataControl;
using System;

namespace Minion.TestConsole.Test1
{
	public class CubePerspective : Indice<int, Company>
	{
		
	}

    public class Company : Indice<DateTime, PayPeriod>
    {
        
    }

    public class PayPeriod : Indice<Guid, Supervisor>
    {
    }

    public class Supervisor : Indice<Guid, Employee>
    {
    }

    public class Employee : IndiceStore<ClockEvent>
    {
    }
}
