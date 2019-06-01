using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using sFndCLIWrapper;

namespace Minion.CP.Abstraction
{
	public interface IAxis
	{
		Task Enable(bool IsEnabled = true);
		Task PrintStatistics();
		Task Home();
		Task Move(Guid correlationId, int position, bool IsAbsolute = false, bool AddDwell = false);

		event Alert OnAlert;
		event MotionComplete OnMotion;
		event StatusUpdate OnStatus;

		long Position { get; }
		bool IsEnabled { get; }
		bool WasHomed { get; }

		void SetBrake(ulong breakNum /*should be ALL right?*/, cliIBrakeControl._BrakeControls mode);
		void SetMotionParameters(double velocity, double acceleration, uint jerk, uint trackLimit, int trackGlobal);
	}

	public class Axis : IAxis, IDisposable
	{
		private readonly IEnumerable<Motor> _motors;

		private double _velocity = 2000;        //VEL_LIM_RPM
		private double _acceleration = 8000;    //ACC_LIM_RPM_PER_SEC
		private uint _jerk = 3;                 //RAS_CODE
		private uint _trackLimit = 100;         //TRACKING_LIM
		private int _trackGlobal = 100;         //TRQ_PCT
		private bool _isQuitting = false;

		public event Alert OnAlert;
		public event MotionComplete OnMotion;
		public event StatusUpdate OnStatus;

		public bool IsEnabled => _motors.All(x => x.Node.Status.IsReady());

		public bool WasHomed => _motors.All(x => x.Node.Motion.Homing.WasHomed());

		public long Position => getPosition(_motors.First());

		public Axis(cliINode[] nodes)
			: this(nodes.Select(x => new Motor(x.Info.Ex.Addr.ToString(), x)))
		{
		}

		//For multiple motors on an axis...
		public Axis(IEnumerable<Motor> motors)
		{
			_motors = motors.ToList();

			initConfiguration();
			initMotionParameters();

			Task.Factory.StartNew(alertWatcher);
		}

		public async Task Enable(bool IsEnabled = true)
		{
			var tasks = _motors
				.Select(x => enableThread(x, IsEnabled))
				.ToArray();

			await Task.WhenAll(tasks);
			await Home();
		}

		public async Task Home()
		{
			var tasks = _motors
				.Select(x => homeThread(x))
				.ToArray();

			await Task.WhenAll(tasks);
		}

		public async Task Move(Guid correlationId, int position, bool IsAbsolute = false, bool AddDwell = false)
		{
			var tasks = _motors.Every(x =>
			{
				x.MoveTimeout = (int)x.Node.Motion.MovePosnDurationMsec(position, IsAbsolute);
				x.MoveTimeout += 20;

				if (!IsAbsolute)
				{
					if (getPosition(x) + position > Int32.MaxValue)
					{
						x.WrapCount++;
					}
					else if (getPosition(x) + position < Int32.MinValue)
					{
						x.WrapCount--;
					}
				}
			})
			.Select(x => moveThread(x, correlationId, position, IsAbsolute, AddDwell))
			.ToArray();

			await Task.WhenAll(tasks);
		}

		public async Task PrintStatistics()
		{
			_motors.Every(x =>
			{
				x.Node.Motion.PosnCommanded.Refresh();
				postStatusEvent(NodeStatus.Statistics, $"[{x.Node.Info.Ex.Addr}] : {x.Name}\t\t{x.MoveCount}\t\t{getPosition(x)}");
			});

			await Task.Delay(10);
		}

		public void SetBrake(ulong breakNum, cliIBrakeControl._BrakeControls mode)
		{
			_motors.Every(x =>
			{
				x.Node.Port.BrakeControl.BrakeSetting(breakNum, mode);
			});
		}

		public void SetMotionParameters(double velocity, double acceleration, uint jerk, uint trackLimit, int trackGlobal)
		{
			_velocity = velocity;
			_acceleration = acceleration;
			_jerk = jerk;
			_trackLimit = trackLimit;
			_trackGlobal = trackGlobal;

			initMotionParameters();
		}

		public void Dispose()
		{
			_isQuitting = true;

			PrintStatistics().Wait();

			var tasks = _motors
				.Every(x =>
				{
					if (!x.Node.Setup.AccessLevelIsFull())
					{
						return;
					}

					Enable(false).Wait();

				})
				.Select(x => disposeThread(x)).ToArray();

			Task.WhenAll(tasks).Wait();
		}

		private void alertWatcher()
		{
			while (!_isQuitting)
			{
				_motors.Every(x =>
				{
					x.Node.Status.Alerts.Refresh();
					x.Node.Status.RT.Refresh();

					if (x.Node.Status.RT.Value().cpm.AlertPresent != 0)
					{
						var alertList = x.Node.Status.Alerts.Value().StateStr();
						postAlertEvent(x, "Node has alerts! Alert: {alertList}");
					}
				});
				Task.Delay(100);
			}
		}

		private async Task disposeThread(Motor motor)
		{
			var watch = new Stopwatch();
			var node = motor.Node;

			do
			{
				await Task.Delay(50);

				if (watch.IsExpired(3000))
				{
					postAlertEvent(motor, "Timed out waiting to disable and dispose");
					break;
				}
			} while (IsEnabled);

			node.Setup.ConfigLoad(motor.ConfigFile, false);
			node.Dispose();
		}

		private async Task enableThread(Motor motor, bool IsEnabled = true)
		{
			var node = motor.Node;
			var statusStart = IsEnabled ? NodeStatus.Enabling : NodeStatus.Disabling;
			var statusDone = IsEnabled ? NodeStatus.Enabled : NodeStatus.Disabled;

			node.Status.AlertsClear();
			node.Motion.NodeStop(cliNodeStopCodes.STOP_TYPE_ABRUPT);
			node.Motion.NodeStop(cliNodeStopCodes.STOP_TYPE_CLR_ALL);

			var watch = new Stopwatch();
			postStatusEvent(statusStart, $"{node.Info.Ex.Addr} {statusStart}");
			node.EnableReq(IsEnabled);

			do
			{
				await Task.Delay(50);
				if (watch.IsExpired(3000))
				{
					postAlertEvent(motor, "Timed out waiting for enable");
				}

			} while (node.Status.IsReady() != IsEnabled);

			postStatusEvent(statusDone, $"{node.Info.Ex.Addr} {statusDone}");
		}

		private long getPosition(Motor motor)
		{
			var node = motor.Node;

			node.Motion.PosnCommanded.Refresh();

			var position = (long)node.Motion.PosnCommanded.Value();

			if (motor.WrapCount != 0)
			{
				position += (Int64)motor.WrapCount << 32;
			}

			return position;
		}

		private async Task homeThread(Motor motor)
		{
			var node = motor.Node;

			if (node.Status.IsReady() && node.Motion.Homing.HomingValid())
			{
				var watch = new Stopwatch();
				postStatusEvent(NodeStatus.Homed, $"{node.Info.Ex.Addr} Homing...");
				node.Motion.Homing.Initiate();

				do
				{
					await Task.Delay(50);
					if (watch.IsExpired(10000))
					{
						postAlertEvent(motor, "Timed out waiting for homing");
						return;
					}

				} while (!node.Motion.Homing.WasHomed());

				postStatusEvent(NodeStatus.Homed, $"{node.Info.Ex.Addr} Homing Complete");
			}
		}

		private void initConfiguration()
		{
			var tempPath = System.IO.Path.GetTempPath();

			_motors.Every(x =>
			{
				x.Node.VelUnit(cliINode._velUnits.RPM);
				x.Node.AccUnit(cliINode._accUnits.RPM_PER_SEC);
				x.Node.Motion.DwellMs.Value(5);

				if (x.Node.Setup.AccessLevelIsFull())
				{
					x.ConfigFile = $"{tempPath}{x.Node.Info.Ex.Addr}{x.Node.Info.SerialNumber.Value()}";
					x.Node.Setup.ConfigSave(x.ConfigFile);
				}

			});
		}

		private void initMotionParameters()
		{
			_motors.Every(x =>
			{
				x.Node.Motion.VelLimit.Value(_velocity);
				x.Node.Motion.AccLimit.Value(_acceleration);
				x.Node.Motion.JrkLimit.Value(_jerk);
				x.Node.Limits.PosnTrackingLimit.Value(_trackLimit);
				x.Node.Limits.TrqGlobal.Value(_trackGlobal);
			});
		}

		public async Task moveThread(Motor motor, Guid correlationId, int position, bool IsAbsolute, bool AddDwell)
		{
			var watch = new Stopwatch();
			var node = motor.Node;
			var addr = node.Info.Ex.Addr.ToString();
			var alerts = new List<string>();

			postMotionEvent(correlationId, false, $"Motor [{motor.Name}] starting its movement");
			postStatusEvent(NodeStatus.Moving, $"Motor [{motor.Name}] starting its movement");

			node.Motion.MovePosnStart(position, IsAbsolute, AddDwell);

			do
			{
				node.Status.RT.Refresh();

				await Task.Delay(50);

				if (watch.IsExpired(motor.MoveTimeout))
				{
					alerts.Add($"Error: [{addr}] : {motor.Name} -> Timed out during move");
				}

				if (node.Status.RT.Value().cpm.Disabled != 0)
				{
					alerts.Add($"Error: [{addr}] : {motor.Name} -> disabled during move");
				}

				if (node.Status.RT.Value().cpm.NotReady != 0)
				{
					alerts.Add($"ERROR: [{addr}] : {motor.Name} -> went NotReady during move");
				}

				if (alerts.Any())
				{
					postAlertEvent(addr, alerts.ToArray());
					return;
				}

			} while (node.Status.RT.Value().cpm.MoveDone != 0);

			postMotionEvent(correlationId, true, $"Motor [{motor.Name}] completed its movement");
			postStatusEvent(NodeStatus.Moved, $"Motor [{motor.Name}] completed its movement");
		}

		private void postAlertEvent(Motor motor, string text)
		{
			postAlertEvent(motor.Node.Info.Ex.Addr.ToString(), new[] { $"Error: [{motor.Node.Info.Ex.Addr}] : {motor.Name} -> {text}" });
		}

		private void postAlertEvent(string address, string[] alerts)
		{
			var args = new AxisAlertEventArgs(address, alerts);
			OnAlert(this, args);
		}

		private void postMotionEvent(Guid correlationId, bool completed, string status)
		{
			var args = new AxisMotionEventArgs(correlationId, completed, status);
			OnMotion(this, args);
		}

		private void postStatusEvent(NodeStatus status, string message)
		{
			var args = new AxisStatusEventArgs(status, message);
			OnStatus(this, args);
		}
	}

	public delegate void Alert(object sender, AxisAlertEventArgs args);
	public delegate void MotionComplete(object sender, AxisMotionEventArgs args);
	public delegate void StatusUpdate(object sender, AxisStatusEventArgs args);
}
