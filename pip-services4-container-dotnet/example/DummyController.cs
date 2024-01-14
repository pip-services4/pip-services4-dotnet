using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Exec;
using PipServices4.Components.Refer;
using PipServices4.Components.Run;
using PipServices4.Observability.Log;
using System.Threading.Tasks;

namespace PipServices4.Container
{
	public sealed class DummyController : IReferenceable, IReconfigurable, IOpenable, INotifiable
	{
		private readonly FixedRateTimer _timer;
		private readonly CompositeLogger _logger = new CompositeLogger();
		public string Message { get; private set; } = "Hello World!";
		public long Counter { get; private set; } = 0;

		public DummyController()
		{
			_timer = new FixedRateTimer(
				async () => { await NotifyAsync(null, new Parameters()); },
				1000, 1000
			);
		}

		public void Configure(ConfigParams config)
		{
			Message = config.GetAsStringWithDefault("message", Message);
		}

		public void SetReferences(IReferences references)
		{
			_logger.SetReferences(references);
		}

		public bool IsOpen()
		{
			return _timer.IsStarted;
		}

		public Task OpenAsync(IContext context)
		{
			_timer.Start();
			_logger.Trace(context, "Dummy controller opened");

			return Task.Delay(0);
		}

		public Task CloseAsync(IContext context)
		{
			_timer.Stop();

			_logger.Trace(context, "Dummy controller closed");

			return Task.Delay(0);
		}

		public Task NotifyAsync(IContext context, Parameters args)
		{
			_logger.Info(context, "{0} - {1}", Counter++, Message);

			return Task.Delay(0);
		}
	}
}
