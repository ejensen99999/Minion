using System;

namespace Minion.TestConsole.Test1
{
    public interface IClockEvent
    {
        int ClockId { get; set; }

        int CompanyId { get; set; }
        string CompanyName { get; set; }

        Guid SupervisorId { get; set; }
        string SupervisorName { get; set; }

        Guid EmployeeId { get; set; }
        string EmployeeName { get; set; }

        DateTime TimeCard { get; set; }
        DateTime Event { get; set; }
        ClockEventTypes Type { get; set; }
    }

    public class ClockEvent: IClockEvent
	{
	    public int ClockId { get; set; }

	    public int CompanyId { get; set; }
	    public string CompanyName { get; set; }

	    public Guid SupervisorId { get; set; }
        public string SupervisorName { get; set; }

        public Guid EmployeeId { get; set; }
	    public string EmployeeName { get; set; }

	    public DateTime TimeCard { get; set; }
		public DateTime Event { get; set; }
		public ClockEventTypes Type { get; set; }
	}
}