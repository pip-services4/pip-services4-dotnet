using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Components.Run;
using PipServices4.Observability.Count;
using PipServices4.Observability.Log;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PipServices4.Rpc.Clients
{
    /// <summary>
    /// Abstract client that calls controller directly in the same memory space.
    /// 
    /// It is used when multiple microservices are deployed in a single container(monolyth)
    /// and communication between them can be done by direct calls rather then through the network.
    /// 
    /// ### Configuration parameters ###
    /// 
    /// dependencies:
    /// - controller:            override controller descriptor
    /// 
    /// ### References ###
    /// 
    /// - *:logger:*:*:1.0         (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_log_1_1_i_logger.html">ILogger</a> components to pass log messages
    /// - *:counters:*:*:1.0         (optional) <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/interface_pip_services_1_1_components_1_1_count_1_1_i_counters.html">ICounters</a> components to pass collected measurements
    /// - *:controller:*:*:1.0     controller to call business methods
    /// </summary>
    /// <typeparam name="T">the class type</typeparam>
    /// <example>
    /// <code>
    /// class MyDirectClient: DirectClient<IMyController>, IMyClient 
    /// {
    ///     public MyDirectClient()
    ///     {   
    ///         base();
    ///         this._dependencyResolver.put('controller', new Descriptor("mygroup", "controller", "*", "*", "*"));
    ///     }
    ///     ...
    ///     
    ///     public MyData GetData(IContext context, string id)
    ///     {
    ///         var timing = this.instrument(context, 'myclient.get_data');
    ///         var result = this._controller.getData(context, id);
    ///         timing.EndTiming();
    ///         return result;
    ///     }
    ///     ...
    /// }
    /// 
    /// var client = new MyDirectClient();
    /// client.SetReferences(References.fromTuples(
    /// new Descriptor("mygroup","controller","default","default","1.0"), controller));
    /// var data = client.GetData("123", "1");
    /// ...
    /// </code>
    /// </example>
    public abstract class DirectClient<T> : IConfigurable, IOpenable, IReferenceable
    {
        /// <summary>
        /// The controller reference.
        /// </summary>
        protected T _controller;
        /// <summary>
        /// The logger.
        /// </summary>
        protected CompositeLogger _logger = new CompositeLogger();
        /// <summary>
        /// The performance counters
        /// </summary>
        protected CompositeCounters _counters = new CompositeCounters();
        /// <summary>
        /// The dependency resolver to get controller reference.
        /// </summary>
        protected DependencyResolver _dependencyResolver = new DependencyResolver();
        /// <summary>
        /// The open flag.
        /// </summary>
        protected bool _opened = false;

        /// <summary>
        /// Creates a new instance of the client.
        /// </summary>
        public DirectClient()
        {
            _dependencyResolver.Put("controller", "none");
        }

        /// <summary>
        /// Configures component by passing configuration parameters.
        /// </summary>
        /// <param name="config">configuration parameters to be set.</param>
        public virtual void Configure(ConfigParams config)
        {
            _dependencyResolver.Configure(config);
        }

        /// <summary>
        /// Sets references to dependent components.
        /// </summary>
        /// <param name="references">references to locate the component dependencies.</param>
        public virtual void SetReferences(IReferences references)
        {
            _logger.SetReferences(references);
            _counters.SetReferences(references);

            _dependencyResolver.SetReferences(references);
            _controller = _dependencyResolver.GetOneRequired<T>("controller");
        }

        /// <summary>
        /// Checks if the component is opened.
        /// </summary>
        /// <returns>true if the component has been opened and false otherwise.</returns>
        public virtual bool IsOpen()
        {
            return _opened;
        }

        /// <summary>
        /// Opens the component.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        public virtual Task OpenAsync(IContext context)
        {
            if (IsOpen())
            {
                return Task.Delay(0);
            }

            if (_controller == null)
            {
                throw new ConnectionException(
                    context != null ? ContextResolver.GetTraceId(context) : null,
                    "NO_CONTROLLER", "Controller reference is missing");
            }

            _logger.Info(context, "Opened Direct client {0}", GetType().Name);

            _opened = true;
            return Task.Delay(0);
        }

        /// <summary>
        /// Closes component and frees used resources.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        public virtual Task CloseAsync(IContext context)
        {
            if (IsOpen())
            {
                _logger.Debug(context, "Closed Direct client {0}", this.GetType().Name);
            }

            _opened = false;

            return Task.Delay(0);
        }

        /// <summary>
        /// Adds instrumentation to log calls and measure call time. It returns a CounterTiming
        /// object that is used to end the time measurement.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="methodName">a method name.</param>
        /// <returns>CounterTiming object to end the time measurement.</returns>
        protected CounterTiming Instrument(IContext context, [CallerMemberName]string methodName = null)
        {
            var typeName = GetType().Name;
            _logger.Trace(context, "Calling {0} method of {1}", methodName, typeName);
            _counters.IncrementOne(typeName + "." + methodName + ".call_count");
            return _counters.BeginTiming(typeName + "." + methodName + ".call_time");
        }

        /// <summary>
        /// Adds instrumentation to error handling.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="methodName">a method name.</param>
        /// <param name="ex">Error that occured during the method call</param>
        /// <param name="rethrow">True to throw the exception</param>
        protected void InstrumentError(IContext context, [CallerMemberName]string methodName = null, Exception ex = null, bool rethrow = false)
        {
            var typeName = GetType().Name;
            _logger.Error(context, ex, "Failed to call {0} method of {1}", methodName, typeName);
            _counters.IncrementOne(typeName + "." + methodName + ".call_errors");

            if (rethrow)
                throw ex;
        }

    }
}
