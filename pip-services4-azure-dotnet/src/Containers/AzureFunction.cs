using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PipServices4.Azure.Controllers;
using PipServices4.Azure.Utils;
using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Data.Validate;
using PipServices4.Observability.Count;
using PipServices4.Observability.Log;
using PipServices4.Observability.Trace;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PipServices4.Azure.Containers
{
    /// <summary>
    /// Abstract Azure Function, that acts as a container to instantiate and run components
    /// and expose them via external entry point.
    /// 
    /// When handling calls "cmd" parameter determines which what action shall be called, while
    /// other parameters are passed to the action itself.
    /// 
    /// Container configuration for this Azure Function is stored in <code>"./config/config.yml"</code> file.
    /// But this path can be overriden by <code>CONFIG_PATH</code> environment variable.
    /// 
    /// ### References ###
    ///     - *:logger:*:*:1.0                              (optional) <see cref="ILogger"/> components to pass log messages
    ///     - *:counters:*:*:1.0                            (optional) <see cref="ICounters"/> components to pass collected measurements
    ///     - *:service:azure-function:*:1.0                  (optional) <see cref="IAzureFunctionService"/> services to handle action requests
    ///     - *:service:commandable-azure-function:*:1.0      (optional) <see cref="IAzureFunctionService"/> services to handle action requests
    /// </summary>
    /// 
    /// <example>
    /// <code>
    /// 
    /// class MyAzureFunction : AzureFunction
    /// {
    ///     public MyAzureFunction() : base("mygroup", "MyGroup Azure Function")
    ///     {
    /// 
    ///     }
    /// }
    /// 
    /// ...
    /// 
    /// var AzureFunction = new MyAzureFunction();
    /// 
    /// await AzureFunction.RunAsync();
    /// Console.WriteLine("MyAzureFunction is started");
    /// 
    /// </code>
    /// </example>
    public abstract class AzureFunction: Container.Containers.Container
    {
        private readonly ManualResetEvent _exitEvent = new ManualResetEvent(false);

        /// <summary>
        /// The performanc counters.
        /// </summary>
        protected CompositeCounters _counters = new CompositeCounters();

        /// <summary>
        /// The tracer.
        /// </summary>
        protected CompositeTracer _tracer = new CompositeTracer();

        /// <summary>
        /// The dependency resolver.
        /// </summary>
        protected DependencyResolver _dependencyResolver = new DependencyResolver();

        /// <summary>
        /// The map of registred validation schemas.
        /// </summary>
        protected Dictionary<string, Schema> _schemas = new();

        /// <summary>
        /// The map of registered actions.
        /// </summary>
        protected Dictionary<string, Func<HttpRequest, Task<IActionResult>>> _actions = new();

        /// <summary>
        /// The default path to config file.
        /// </summary>
        protected string _configPath = "../config/config.yml";

        /// <summary>
        /// Creates a new instance of this Azure Function function.
        /// </summary>
        /// <param name="name">(optional) a container name (accessible via ContextInfo)</param>
        /// <param name="descriptor">(optional) a container description (accessible via ContextInfo)</param>
        public AzureFunction(string name, string descriptor): base(name, descriptor)
        {
            _logger = new ConsoleLogger();
        }

        private string GetConfigPath()
        {
            return Environment.GetEnvironmentVariable("CONFIG_PATH") ?? this._configPath;
        }

        private ConfigParams GetParameters()
        {
            return ConfigParams.FromValue(Environment.GetEnvironmentVariables());
        }

        private void CaptureErrors(IContext context)
        {
            AppDomain.CurrentDomain.UnhandledException += (obj, e) =>
            {
                _logger.Fatal(context, e.ExceptionObject.ToString(), "Process is terminated");
                _exitEvent.Set();
            };
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
        /// Sets references to dependent components.
        /// </summary>
        /// <param name="references">references to locate the component dependencies. </param>
        public override void SetReferences(IReferences references)
        {
            base.SetReferences(references);
            _counters.SetReferences(references);
            _dependencyResolver.SetReferences(references);

            Register();
        }

        /// <summary>
        /// Opens the component.
        /// </summary>
        /// <param name="context">(optional) transaction id to trace execution through call chain.</param>
        public new async Task OpenAsync(IContext context)
        {
            if (IsOpen()) return;

            await base.OpenAsync(context);
            RegisterServices();
        }

        /// <summary>
        /// Adds instrumentation to log calls and measure call time. It returns a CounterTiming
        /// object that is used to end the time measurement.
        /// </summary>
        /// <param name="context">(optional) transaction id to trace execution through call chain.</param>
        /// <param name="methodName">a method name.</param>
        /// <returns>CounterTiming object to end the time measurement.</returns>
        protected CounterTiming Instrument(IContext context, string methodName)
        {
            _logger.Trace(context, "Executing {0} method", methodName);
            _counters.IncrementOne(methodName + ".exec_count");
            return _counters.BeginTiming(methodName + ".exec_time");
        }

        /// <summary>
        /// Adds instrumentation to error handling.
        /// </summary>
        /// <param name="context">(optional) transaction id to trace execution through call chain.</param>
        /// <param name="methodName">a method name.</param>
        /// <param name="ex">Error that occured during the method call</param>
        /// <param name="rethrow">True to throw the exception</param>
        protected void InstrumentError(IContext context, string methodName, Exception ex, bool rethrow = false)
        {
            _logger.Error(context, ex, "Failed to execute {0} method", methodName);
            _counters.IncrementOne(methodName + ".exec_errors");

            if (rethrow)
                throw ex;
        }

        /// <summary>
        /// Runs this Azure Function, loads container configuration,
        /// instantiate components and manage their lifecycle,
        /// makes this function ready to access action calls.
        /// </summary>
        public async Task RunAsync()
        {
            var context = Context.FromTraceId(_info.Name);
            var path = GetConfigPath();
            var parameters = GetParameters();
            ReadConfigFromFile(context, path, parameters);

            CaptureErrors(context);
            await OpenAsync(context);
            CaptureExit(context);
            await CloseAsync(context);
        }

        /// <summary>
        /// Registers all actions in this Azure Function.
        /// 
        /// Note: Overloading of this method has been deprecated. Use <see cref="AzureFunctionController"/> instead.
        /// </summary>
        [Obsolete("Overloading of this method has been deprecated. Use AzureFunctionService instead.", false)]
        protected virtual void Register() { }

        /// <summary>
        /// Registers all Azure Function services in the container.
        /// </summary>
        protected void RegisterServices()
        {
            // Extract regular and commandable Azure Function services from references
            var services = this._references.GetOptional<IAzureFunctionController>(
                new Descriptor("*", "controller", "azure-function", "*", "*")
            );
            var cmdServices = this._references.GetOptional<IAzureFunctionController>(
                new Descriptor("*", "controller", "commandable-azure-function", "*", "*")
            );

            services.AddRange(cmdServices);

            // Register actions defined in those services
            foreach (var service in services)
            {

                var actions = service.GetActions();
                foreach (var action in actions)
                {
                    RegisterAction(action.Cmd, action.Schema, action.Action);
                }
            }
        }

        /// <summary>
        /// Registers an action in this Azure Function.
        /// 
        /// Note: This method has been deprecated. Use <see cref="AzureFunctionController"/> instead.
        /// </summary>
        /// <param name="cmd">a action/command name.</param>
        /// <param name="schema">a validation schema to validate received parameters.</param>
        /// <param name="action">an action function that is called when action is invoked.</param>
        /// <exception cref="UnknownException"></exception>
        [Obsolete("This method has been deprecated. Use AzureFunctionService instead.", false)]
        protected void RegisterAction(string cmd, Schema schema, Func<HttpRequest, Task<IActionResult>> action)
        {
            if (string.IsNullOrEmpty(cmd))
                throw new UnknownException(null, "NO_COMMAND", "Missing command");

            if (action == null)
                throw new UnknownException(null, "NO_ACTION", "Missing action");

            if (this._actions.ContainsKey(cmd))
                throw new UnknownException(null, "DUPLICATED_ACTION", cmd + "action already exists");

            Func<HttpRequest, Task<IActionResult>> actionCurl = async (req) =>
            {
                // Perform validation
                if (schema != null)
                {
                    var param = AzureFunctionContextHelper.GetParameters(req);
                    var context = GetTraceId(req);
                    var err = schema.ValidateAndReturnException(context, param, false);
                    if (err != null)
                        return AzureFunctionResponseSender.SendErrorAsync(err);
                }

                return await action(req);
            };

            _actions[cmd] = actionCurl;
        }

        /// <summary>
        /// Returns context from Azure Function request.
        /// This method can be overloaded in child classes
        /// </summary>
        /// <param name="request">Azure Function request</param>
        /// <returns>Returns context from request</returns>
        protected string GetTraceId(HttpRequest request)
        {
            return AzureFunctionContextHelper.GetTraceId(request);
        }

        /// <summary>
        /// Returns command from Azure Function request.
        /// This method can be overloaded in child classes
        /// </summary>
        /// <param name="request">Azure Function request</param>
        /// <returns>Returns command from request</returns>
        protected string GetCommand(HttpRequest request)
        {
            return AzureFunctionContextHelper.GetCommand(request);
        }

        /// <summary>
        /// Executes this Azure Function and returns the result.
        /// This method can be overloaded in child classes
        /// if they need to change the default behavior
        /// </summary>
        /// <param name="req">the request function</param>
        /// <returns>task</returns>
        /// <exception cref="BadRequestException"></exception>
        protected async Task<IActionResult> ExecuteAsync(HttpRequest req)
        {
            string cmd = GetCommand(req);
            var traceId = GetTraceId(req);

            if (string.IsNullOrEmpty(cmd))
            {
                throw new BadRequestException(
                    traceId,
                    "NO_COMMAND",
                    "Cmd parameter is missing"
                );
            }

            var action = this._actions[cmd];
            if (action == null)
            {
                throw new BadRequestException(
                    traceId,
                    "NO_ACTION",
                    "Action " + cmd + " was not found"
                )
                .WithDetails("command", cmd);
            }

            return await action(req);
        }

        private async Task<IActionResult> Handler(HttpRequest req)
        {
            // If already started then execute
            if (IsOpen())
                return await ExecuteAsync(req);
            // Start before execute
            await RunAsync();
            return await ExecuteAsync(req);
        }

        /// <summary>
        /// Gets entry point into this Azure Function.
        /// </summary>
        /// <returns>Returns plugin function</returns>
        public Func<HttpRequest, Task<IActionResult>> GetHandler()
        {
            return Handler;
        }
    }
}