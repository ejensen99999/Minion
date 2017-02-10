namespace Minion.Security
{
	public interface IAudit
	{
		void Record(AuditData data);
	}
}