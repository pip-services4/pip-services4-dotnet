using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Run;
using PipServices4.Data.Random;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PipServices4.Container.Test
{
	/// <summary>
	/// Random shutdown component that crashes the process
	/// using various methods.
	/// The component is usually used for testing, but brave developers
	/// can try to use it in production to randomly crash microservices.
	/// It follows the concept of "Chaos Monkey" popularized by Netflix.
	/// 
	/// ### Configuration parameters ###
	/// 
	/// - mode:          null - crash by NullPointer excepiton, zero - crash by dividing by zero, excetion = crash by unhandled exception, exit - exit the process
	/// - min_timeout:   minimum crash timeout in milliseconds(default: 5 mins)
	/// - max_timeout:   maximum crash timeout in milliseconds(default: 15 minutes)
	/// 
	/// <example>
	/// <code>
	/// var shutdown = new Shutdown();
	/// shutdown.Configure(ConfigParams.FromTuples(
	///         "mode", "exception"
	/// ));
	/// 
	/// shutdown.Shutdowns();         // Result: Bang!!! the process crashes
	/// </code>
	/// </example>
	/// </summary>
	public class Shutdown : IConfigurable, IOpenable
	{
		private Timer _timer = null;
		private string _mode = "exception";
		private int _minTimeout = 300000;
		private int _maxTimeout = 900000;


		public void Configure(ConfigParams config)
		{
			this._mode = config.GetAsStringWithDefault("mode", this._mode);
			this._minTimeout = config.GetAsIntegerWithDefault("min_timeout", this._minTimeout);
			this._maxTimeout = config.GetAsIntegerWithDefault("max_timeout", this._maxTimeout);
		}

		public bool IsOpen()
		{
			return _timer != null;
		}

		public async Task OpenAsync(string correlationId)
		{
			if (_timer != null)
				await Task.Run(() => _timer.Dispose());

			var timeout = RandomInteger.NextInteger(this._minTimeout, this._maxTimeout);

			TimerCallback callback = new TimerCallback((object state) => Shutdowns());
			_timer = new Timer(callback, null, 0, timeout);
		}

		public async Task CloseAsync(string correlationId)
		{
			if (_timer != null)
			{
				await Task.Run(() => _timer.Dispose());
				this._timer = null;
			}

		}

		public void Shutdowns()
		{
			if (this._mode == "null" || this._mode == "nullpointer")
			{
				throw new NullReferenceException("nullpointer");
			}
			else if (this._mode == "zero" || this._mode == "dividebyzero")
			{
				var crash = 0 / 100;
			}
			else if (this._mode == "exit" || this._mode == "processexit")
			{
				Environment.Exit(1);
			}
			else
			{
				var err = ApplicationExceptionFactory.Create(new ErrorDescription() { Category = "test", Message = "Crash test exception" });
				throw err;
			}
		}
	}
}
