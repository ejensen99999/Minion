using System;
using sFndCLIWrapper;

namespace Minion.CP.Abstraction
{
	public interface IMotor
	{
		int MoveCount { get; set; }
		int WrapCount { get; set; }
		int MoveTimeout { get; set; }

		string Name { get; }
		string ConfigFile { get; }

		string Address { get; }
		bool CanHome { get; }
		bool IsFullAccess { get; }
		bool IsEnabled { get; }
		bool WasHomed { get; }
		long Position { get; }
		MoveStatus MoveStatus { get; }

		void Configure();
		void Enable(bool shouldEnable = true);
		string GetAlerts();
		int GetMoveTime(int position, bool IsAbsolute);
		void Home();
		void Move(int position, bool IsAbsolute, bool AddDwell);
		void SetMotionParameters();
		void SetMotionParameters(double velocity, double acceleration, uint jerk, uint trackLimit, int trackGlobal);
		void Dispose();
	}

	public class Motor : IMotor, IDisposable //Node Abstraction
	{
		private readonly cliINode _node;

		
		public int MoveCount { get; set; } = 0;
		public int WrapCount { get; set; } = 0;
		public int MoveTimeout { get; set; } = 0;

		public string Name { get; }
		public string ConfigFile { get; }

		public Motor(cliINode node, string name, string configFile)
		{
			_node = node;

			Name = name;
			ConfigFile = $"{configFile}{Address}{_node.Info.SerialNumber.Value()}";
		}

		public string Address
		{
			get { return _node.Info.Ex.Addr.ToString(); }
		}

		public bool CanHome
		{
			get { return _node.Motion.Homing.HomingValid(); }
		}

		public bool IsFullAccess
		{
			get { return _node.Setup.AccessLevelIsFull(); }
		}

		public bool IsEnabled
		{
			get { return _node.Status.IsReady(); }
		}

		public MoveStatus MoveStatus
		{
			get
			{
				_node.Status.RT.Refresh();
				var output = MoveStatus.Moving;

				if (_node.Status.RT.Value().cpm.Disabled != 0)
				{
					output = MoveStatus.Disabled;
				}
				else if (_node.Status.RT.Value().cpm.NotReady != 0)
				{
					output = MoveStatus.NotReady;
				}
				else if (_node.Status.RT.Value().cpm.MoveDone != 0)
				{
					output = MoveStatus.Done;
				}

				return output;
			}
		}

		public long Position
		{
			get
			{
				_node.Motion.PosnCommanded.Refresh();
				return (long)_node.Motion.PosnCommanded.Value();
			}
		}

		public bool WasHomed
		{
			get { return _node.Motion.Homing.WasHomed(); }
		}

		public void Configure()
		{
			_node.VelUnit(cliINode._velUnits.RPM);
			_node.AccUnit(cliINode._accUnits.RPM_PER_SEC);
			_node.Motion.DwellMs.Value(5);

			if (_node.Setup.AccessLevelIsFull())
			{
				_node.Setup.ConfigSave(ConfigFile);
			}
		}

		public void Enable(bool shouldEnable = true)
		{
			_node.Status.AlertsClear();
			_node.Motion.NodeStop(cliNodeStopCodes.STOP_TYPE_ABRUPT);
			_node.Motion.NodeStop(cliNodeStopCodes.STOP_TYPE_CLR_ALL);
			_node.EnableReq(shouldEnable);
		}

		public string GetAlerts()
		{
			string output = null;

			_node.Status.Alerts.Refresh();
			_node.Status.RT.Refresh();

			if (_node.Status.RT.Value().cpm.AlertPresent != 0)
			{
				output = _node.Status.Alerts.Value().StateStr();
			}

			return output;
		}

		public int GetMoveTime(int position, bool IsAbsolute)
		{
			return (int)_node.Motion.MovePosnDurationMsec(position, IsAbsolute);
		}

		public void Home()
		{
			_node.Motion.Homing.Initiate();
		}

		public void Move(int position, bool IsAbsolute, bool AddDwell)
		{
			_node.Motion.MovePosnStart(position, IsAbsolute, AddDwell);
		}

		public void SetMotionParameters()
		{
			SetMotionParameters(2000, 8000, 3, 100, 100);
		}

		public void SetMotionParameters(double velocity, double acceleration, uint jerk, uint trackLimit, int trackGlobal)
		{
			_node.Motion.VelLimit.Value(velocity);
			_node.Motion.AccLimit.Value(acceleration);
			_node.Motion.JrkLimit.Value(jerk);
			_node.Limits.PosnTrackingLimit.Value(trackLimit);
			_node.Limits.TrqGlobal.Value(trackGlobal);
		}

		public void Dispose()
		{
			_node.Setup.ConfigLoad(ConfigFile, false);
			_node.Dispose();
		}
	}
}
