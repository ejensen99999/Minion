using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

		//void SetBrake(ulong breakNum, cliIBrakeControl._BrakeControls mode);
		void SetMotionParameters(double velocity, double acceleration, uint jerk, uint trackLimit, int trackGlobal);
	}

	public class Axis : IAxis, IDisposable
	{
		private readonly IEnumerable<Motor> _motors;

		private bool _isQuitting = false;

		public event Alert OnAlert;
		public event MotionComplete OnMotion;
		public event StatusUpdate OnStatus;

		public bool IsEnabled => _motors.All(x => x.IsEnabled);

		public bool WasHomed => _motors.All(x => x.WasHomed);

		public long Position => getPosition(_motors.First());

		private long getPosition(Motor motor)
		{
			var position = motor.Position;

			if (motor.WrapCount != 0)
			{
				position += (long)motor.WrapCount << 32;
			}

			return position;
		}

		public Axis(cliINode[] nodes)
			: this(nodes.Select(x => new Motor(x, x.Info.Ex.Addr.ToString(), Path.GetTempPath())))
		{
		}

		//For multiple motors on an axis...
		public Axis(IEnumerable<Motor> motors)
		{
			_motors = motors.ToList();

			initConfiguration();
			SetMotionParameters();

			Task.Factory.StartNew(alertWatcher);
		}

		public async Task Enable(bool IsEnabled = true)
		{
			var tasks = _motors
				.Select(x => enableTaskAsync(x, IsEnabled))
				.ToArray();

			await Task.WhenAll(tasks);
			await Home();
		}

		private async Task enableTaskAsync(Motor motor, bool shouldEnable = true)
		{
			//var node = motor.Node;
			var address = motor.Address;
			var statusStart = shouldEnable ? NodeStatus.Enabling : NodeStatus.Disabling;
			var statusDone = shouldEnable ? NodeStatus.Enabled : NodeStatus.Disabled;

			motor.Enable();

			var watch = new Stopwatch();
			postStatusEvent(statusStart, $"{address} {statusStart}");

			do
			{
				await Task.Delay(50);
				if (watch.IsExpired(3000))
				{
					postAlertEvent(motor, "Timed out waiting for enable");
				}

			} while (motor.IsEnabled != shouldEnable);

			postStatusEvent(statusDone, $"{address} {statusDone}");
		}

		public async Task Home()
		{
			var tasks = _motors
				.Select(x => homeTaskAsync(x))
				.ToArray();

			await Task.WhenAll(tasks);
		}

		private async Task homeTaskAsync(Motor motor)
		{
			if (motor.IsEnabled && motor.CanHome)
			{
				var watch = new Stopwatch();
				postStatusEvent(NodeStatus.Homed, $"{motor.Address} Homing...");

				motor.Home();

				do
				{
					await Task.Delay(50);
					if (watch.IsExpired(10000))
					{
						postAlertEvent(motor, "Timed out waiting for homing");
						return;
					}

				} while (!motor.WasHomed);

				postStatusEvent(NodeStatus.Homed, $"{motor.Address} Homing Complete");
			}
		}

		public async Task Move(Guid correlationId, int position, bool IsAbsolute = false, bool AddDwell = false)
		{
			var tasks = _motors.ForAll(x =>
			{
				x.MoveTimeout = x.GetMoveTime(position, IsAbsolute);
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
			.Select(x => moveTaskAsync(x, correlationId, position, IsAbsolute, AddDwell))
			.ToArray();

			await Task.WhenAll(tasks);
		}

		public async Task moveTaskAsync(Motor motor, Guid correlationId, int position, bool IsAbsolute, bool AddDwell)
		{
			var watch = new Stopwatch();
			var address = motor.Address;
			var alerts = new List<string>();

			postMotionEvent(correlationId, false, $"Motor [{motor.Name}] starting its movement");
			postStatusEvent(NodeStatus.Moving, $"Motor [{motor.Name}] starting its movement");

			motor.Move(position, IsAbsolute, AddDwell);
			var status = MoveStatus.Moving;

			do
			{
				await Task.Delay(50);

				status = motor.MoveStatus;

				if (watch.IsExpired(motor.MoveTimeout))
				{
					alerts.Add($"Error: [{address}] : {motor.Name} -> Timed out during move");
				}

				if (status == MoveStatus.Disabled)
				{
					alerts.Add($"Error: [{address}] : {motor.Name} -> disabled during move");
				}

				if (status == MoveStatus.NotReady)
				{
					alerts.Add($"ERROR: [{address}] : {motor.Name} -> went NotReady during move");
				}

				if (alerts.Any())
				{
					postAlertEvent(address, alerts.ToArray());
					return;
				}

			} while (status != MoveStatus.Done);

			postMotionEvent(correlationId, true, $"Motor [{motor.Name}] completed its movement");
			postStatusEvent(NodeStatus.Moved, $"Motor [{motor.Name}] completed its movement");
		}

		public async Task PrintStatistics()
		{
			_motors.ForAll(x =>
			{
				postStatusEvent(NodeStatus.Statistics, $"[{x.Address}] : {x.Name}\t\t{x.MoveCount}\t\t{getPosition(x)}");
			});

			await Task.Delay(10);
		}

		//If this is part of the Port why here?
		//public void SetBrake(ulong breakNum, cliIBrakeControl._BrakeControls mode)
		//{
		//	_motors.ForAll(x =>
		//	{
		//		x.Node.Port.BrakeControl.BrakeSetting(breakNum, mode);
		//	});
		//}

		public void SetMotionParameters()
		{
			_motors.ForAll(x =>
			{
				x.SetMotionParameters();
			});
		}

		public void SetMotionParameters(double velocity, double acceleration, uint jerk, uint trackLimit, int trackGlobal)
		{
			_motors.ForAll(x =>
			{
				x.SetMotionParameters(velocity, acceleration, jerk, trackLimit, trackGlobal);
			});
		}

		public void Dispose()
		{
			_isQuitting = true;

			PrintStatistics().Wait();

			var tasks = _motors
				.ForAll(x =>
				{
					if (!x.IsFullAccess)
					{
						return;
					}

					Enable(false).Wait();

				})
				.Select(x => disposeTaskAsync(x)).ToArray();

			Task.WhenAll(tasks).Wait();
		}

		private async Task disposeTaskAsync(Motor motor)
		{
			var watch = new Stopwatch();
			do
			{
				await Task.Delay(50);

				if (watch.IsExpired(3000))
				{
					postAlertEvent(motor, "Timed out waiting to disable and dispose");
					break;
				}
			} while (IsEnabled);

			motor.Dispose();
		}

		private void alertWatcher()
		{
			while (!_isQuitting)
			{
				_motors.ForAll(x =>
				{
					var alerts = x.GetAlerts();

					if (!string.IsNullOrWhiteSpace(alerts))
					{
						postAlertEvent(x, $"Node has alerts! Alert: {alerts}");
					}
				});
				Task.Delay(100);
			}
		}

		private void initConfiguration()
		{
			_motors.ForAll(x =>
			{
				x.Configure();

			});
		}

		private void postAlertEvent(Motor motor, string text)
		{
			var addr = motor.Address;
			postAlertEvent(addr, new[] { $"Error: [{addr}] : {motor.Name} -> {text}" });
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
