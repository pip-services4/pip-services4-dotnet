using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Observability.Log;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PipServices4.Container.Containers
{
	/// <summary>
	/// Inversion of control (IoC) container that runs as a system process.
	/// It processes command line arguments and handles unhandled exceptions and Ctrl-C signal
	/// to gracefully shutdown the container.
	/// 
	/// ### Command line arguments ###
	/// - <code>--config / -c</code> path to JSON or YAML file with container configuration (default: "./config/config.yml")
	/// - <code>--param / --params / -p</code> value(s) to parameterize the container configuration 
	/// - <code>--help / -h</code> prints the container usage help
	/// </summary>
	/// <example>
	/// <code>
	/// var container = new ProcessContainer();
	/// container.AddFactory(new MyComponentFactory());
	/// 
	/// container.RunAsync(process.getArgs());
	/// </code>
	/// </example>
	public class ProcessContainer : Container
	{
		protected string _configPath = "../config/config.yml";
		private readonly ManualResetEvent _exitEvent = new ManualResetEvent(false);

		/// <summary>
		/// Creates a new instance of the container.
		/// </summary>
		/// <param name="name">(optional) a container name (accessible via ContextInfo)</param>
		/// <param name="description">(optional) a container description (accessible via ContextInfo)</param>
		public ProcessContainer(string name = null, string description = null)
			: base(name, description)
		{
			_logger = new ConsoleLogger();
		}

		private string GetConfigPath(string[] args)
		{
			for (var index = 0; index < args.Length; index++)
			{
				var arg = args[index];
				var nextArg = index < args.Length - 1 ? args[index + 1] : null;
				nextArg = nextArg != null && nextArg.StartsWith("-", StringComparison.InvariantCulture) ? null : nextArg;
				if (nextArg != null)
				{
					if (arg == "--config" || arg == "-c")
					{
						return nextArg;
					}
				}
			}
			return _configPath;
		}

		private ConfigParams GetParameters(string[] args)
		{
			// Process command line parameters
			var line = "";
			for (var index = 0; index < args.Length; index++)
			{
				var arg = args[index];
				var nextArg = index < args.Length - 1 ? args[index + 1] : null;
				nextArg = nextArg != null && nextArg.StartsWith("-", StringComparison.InvariantCulture) ? null : nextArg;
				if (nextArg != null)
				{
					if (arg == "--param" || arg == "--params" || arg == "-p")
					{
						if (line.Length > 0)
							line = line + ';';
						line = line + nextArg;
						index++;
					}
				}
			}
			var parameters = ConfigParams.FromString(line);

			// Process environmental variables
			foreach (var key in Environment.GetEnvironmentVariables().Keys)
			{
				var name = key.ToString();
				var value = Environment.GetEnvironmentVariable(name);
				parameters.Set(name, value);
			}

			return parameters;
		}

		private bool ShowHelp(string[] args)
		{
			for (var index = 0; index < args.Length; index++)
			{
				var arg = args[index];
				if (arg == "--help" || arg == "-h")
					return true;
			}
			return false;
		}

		private void PrintHelp()
		{
			Console.Out.WriteLine("Pip.Services process container - http://www.github.com/pip-services4/pip-services4");
			Console.Out.WriteLine("run [-h] [-c <config file>] [-p <param>=<value>]*");
		}

		//public object AppDomain { get; private set; }
		private void CaptureErrors(IContext context)
		{
			//AppDomain.CurrentDomain.UnhandledException += (obj, e) =>
			//{
			//    _logger.Fatal(context, e.ExceptionObject, "Process is terminated");
			//    _exitEvent.Set();
			//};
		}

		private void CaptureExit(IContext context)
		{
			_logger.Info(context, "Press Control-C to stop the microservice...");

			Console.CancelKeyPress += (sender, eventArgs) =>
			{
				_logger.Info(context, "Goodbye!");

				eventArgs.Cancel = true;
				_exitEvent.Set();

				Environment.Exit(1);
			};

			// Wait and close
			_exitEvent.WaitOne();
		}

		/// <summary>
		/// Runs the container by instantiating and running components inside the container.
		/// 
		/// It reads the container configuration, creates, configures, references and
		/// opens components.On process exit it closes, unreferences and destroys
		/// components to gracefully shutdown.
		/// </summary>
		/// <param name="args">command line arguments</param>
		public async Task RunAsync(string[] args)
		{
			if (ShowHelp(args))
			{
				PrintHelp();
				return;
			}

			var context = Context.FromTraceId(_info.Name);
			var path = GetConfigPath(args);
			var parameters = GetParameters(args);
			this.ReadConfigFromFile(context, path, parameters);

			CaptureErrors(context);
			await OpenAsync(context);
			CaptureExit(context);
			await CloseAsync(context);
		}

	}
}
