using PipServices4.Components.Config;
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

		public Task OpenAsync(string correlationId)
		{
			_timer.Start();
			_logger.Trace(correlationId, "Dummy controller opened");

			return Task.Delay(0);
		}

		public Task CloseAsync(string correlationId)
		{
			_timer.Stop();

			_logger.Trace(correlationId, "Dummy controller closed");

			return Task.Delay(0);
		}

		public Task NotifyAsync(string correlationId, Parameters args)
		{
			_logger.Info(correlationId, "{0} - {1}", Counter++, Message);

			return Task.Delay(0);
		}
	}
}
