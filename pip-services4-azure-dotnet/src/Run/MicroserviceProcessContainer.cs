using Microsoft.ServiceFabric.Services.Runtime;
using PipServices4.Commons.Errors;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Components.Run;
using PipServices4.Container.Refer;
using PipServices4.Observability.Log;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PipServices4.Azure.Run
{
    /// <summary>
    /// Class MicroserviceProcessContainer.
    /// </summary>
    public class MicroserviceProcessContainer : Container.Containers.Container
    {
        private IContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroserviceProcessContainer"/> class.
        /// </summary>
        public MicroserviceProcessContainer()
        {
            _references = new ContainerReferences();
        }

        private void CaptureErrors()
        {
            AppDomain.CurrentDomain.UnhandledException += HandleUncaughtException;
        }

        private void HandleUncaughtException(object sender, UnhandledExceptionEventArgs args)
        {
            _logger.Fatal(_context, (Exception)args.ExceptionObject, "Process is terminated");
        }

        private async Task RunAsync(IContext context, CancellationToken token)
        {
            _context = context;

            CaptureErrors();

            //await StartAsync(context, token);

            if (_config == null)
                throw new InvalidStateException(context != null ? ContextResolver.GetTraceId(context) : null, 
                    "NO_CONFIG", "Container was not configured");

            try
            {
                _logger.Trace(context, "Starting container.");

                // Create references with configured components
                InitReferences(_references);
                _references.PutFromConfig(_config);

                // Reference and open components
                var components = _references.GetAll();
                Referencer.SetReferences(_references, components);
                await Opener.OpenAsync(context, _references.GetAll());

                // Get reference to logger
                _logger = new CompositeLogger(_references);

                // Get reference to container info
                var infoDescriptor = new Descriptor("*", "container-info", "*", "*", "*");
                _info = (ContextInfo)_references.GetOneRequired(infoDescriptor);

                _logger.Info(context, "Container {0} started.", _info.Name);
            }
            catch (Exception ex)
            {
                _references = null;
                _logger.Error(context, ex, "Failed to start container");

                throw;
            }
        }

        /// <summary>
        /// stop as an asynchronous operation.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        public async Task StopAsync(CancellationToken token)
        {
            await StopAsync(token);
        }


        /// <summary>
        /// Runs the with configuration file asynchronous.
        /// </summary>
        /// <param name="context">The correlation identifier.</param>
        /// <param name="path">The path.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        public Task RunWithConfigFileAsync(IContext context, string path, CancellationToken token)
        {
            ReadConfigFromFile(context, path, null);
            return RunAsync(context, token);
        }

        /// <summary>
        /// Gets stateless service.
        /// </summary>
        /// <returns>StatelessService.</returns>
        public T GetService<T>()
            where T: class
        {
            if ( !typeof(StatelessService).IsAssignableFrom(typeof(T)) && !typeof(StatefulService).IsAssignableFrom(typeof(T)))
                throw new ArgumentException("Service should be derived from either StatelessService or StatefulService", nameof(T));

            return _references.GetOneRequired<T>(new Descriptor("*", "service", "azure-stateless", "*", "*"));
        }
    }
}
