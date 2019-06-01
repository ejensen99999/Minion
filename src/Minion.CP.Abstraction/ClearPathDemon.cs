using sFndCLIWrapper;

namespace Minion.CP.Abstraction
{
	public class ClearPathDemon
	{
		private readonly cliSysMgr _manager;

		public Axis X { get; }

		public ClearPathDemon(cliSysMgr manager)
		{
			_manager = manager;

			X = new Axis(new[] { _manager.Ports(1).Nodes(1) });
			X.OnAlert += X_OnAlert;
		}

		private void X_OnAlert(object sender, AxisAlertEventArgs args)
		{
			throw new System.NotImplementedException();
		}

		public ClearPathDemon()
			: this(new cliSysMgr())
		{
		}
	}
}
