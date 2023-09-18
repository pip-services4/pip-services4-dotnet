using PipServices4.Commons.Errors;
using PipServices4.Components.Build;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Components.Run;
using PipServices4.Container.Build;
using PipServices4.Container.Config;
using PipServices4.Container.Refer;
using PipServices4.Observability.Log;
using System;
using System.Threading.Tasks;

namespace PipServices4.Container.Containers
{
	/// <summary>
	/// Inversion of control (IoC) container that creates components and manages their lifecycle.
	/// 
	/// The container is driven by configuration, that usually stored in JSON or YAML file.
	/// The configuration contains a list of components identified by type or locator, followed
	/// by component configuration.
	/// 
	/// On container start it performs the following actions:
	/// - Creates components using their types or calls registered factories to create components using their locators
	/// - Configures components that implement <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/interface_pip_services3_1_1_commons_1_1_config_1_1_i_configurable.html">IConfigurable</a> interface and passes them their configuration parameters
	/// - Sets references to components that implement <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/interface_pip_services3_1_1_commons_1_1_refer_1_1_i_referenceable.html">IReferenceable</a> interface and passes them references of all components in the container
	/// - Opens components that implement <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/interface_pip_services3_1_1_commons_1_1_run_1_1_i_openable.html">IOpenable</a> interface
	/// 
	/// On container stop actions are performed in reversed order:
	/// - Closes components that implement <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/interface_pip_services3_1_1_commons_1_1_run_1_1_i_closable.html">IClosable</a> interface
	/// - Unsets references in components that implement <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/interface_pip_services3_1_1_commons_1_1_refer_1_1_i_unreferenceable.html">IUnreferenceable</a> interface
	/// - Destroys components in the container.
	/// 
	/// The component configuration can be parameterized by dynamic values.That allows specialized containers
	/// to inject parameters from command line or from environment variables.
	/// 
	/// The container automatically creates a <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/class_pip_services_1_1_components_1_1_info_1_1_context_info.html">ContextInfo</a> component that carries detail information
	/// about the container and makes it available for other components.
	/// 
	/// ### Configuration parameters ###
	/// 
	/// - name: 					the context (container or process) name
	/// - description: 		   	human-readable description of the context
	/// - properties: 			    entire section of additional descriptive properties
	/// - ...
	/// </summary>
	/// <example>
	/// <code>
	/// ======= config.yml ========
	/// - descriptor: mygroup:mycomponent1:default:default:1.0
	/// param1: 123
	/// param2: ABC
	/// 
	/// - type: mycomponent2,mypackage
	/// param1: 321
	/// param2: XYZ
	/// ============================
	/// 
	/// var container = new Container();
	/// container.AddFactory(new MyComponentFactory());
	/// 
	/// var parameters = ConfigParams.fromValue(process.env);
	/// container.ReadConfigFromFile("123", "./config/config.yml", parameters);
	/// 
	/// container.Open("123");
	/// Console.Out.WriteLine("Container is opened");
	/// ...
	/// container.Close("123");
	/// Console.Out.WriteLine("Container is closed");
	/// </code>
	/// </example>
	/// See <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/interface_pip_services3_1_1_commons_1_1_config_1_1_i_configurable.html">IConfigurable</a>, 
	/// <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/interface_pip_services3_1_1_commons_1_1_refer_1_1_i_referenceable.html">IReferenceable</a>, 
	/// <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/interface_pip_services3_1_1_commons_1_1_run_1_1_i_openable.html">IOpenable</a>
	public class Container : IConfigurable, IReferenceable, IUnreferenceable, IOpenable
	{
		protected ILogger _logger = new NullLogger();
		protected DefaultContainerFactory _factories = new DefaultContainerFactory();
		protected ContextInfo _info;
		protected ContainerConfig _config;
		protected ContainerReferences _references;

		/// <summary>
		/// Creates a new instance of the container.
		/// </summary>
		/// <param name="name">(optional) a container name (accessible via ContextInfo)</param>
		/// <param name="description">(optional) a container description (accessible via ContextInfo)</param>
		public Container(string name = null, string description = null)
		{
			_info = new ContextInfo(name, description);
		}

		/// <summary>
		/// Configures component by passing configuration parameters.
		/// </summary>
		/// <param name="config">configuration parameters to be set.</param>
		public virtual void Configure(ConfigParams config)
		{
			_config = ContainerConfig.FromConfig(config);
		}

		/// <summary>
		/// Reads container configuration from JSON or YAML file and parameterizes it with given values.
		/// </summary>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="path">a path to configuration file</param>
		/// <param name="parameters">values to parameters the configuration or null to skip parameterization.</param>
		public void ReadConfigFromFile(string correlationId, string path, ConfigParams parameters)
		{
			_config = ContainerConfigReader.ReadFromFile(correlationId, path, parameters);
		}

		/// <summary>
		/// Sets references to dependent components.
		/// </summary>
		/// <param name="references">references to locate the component dependencies.</param>
		public virtual void SetReferences(IReferences references)
		{
			// Override in child class
		}

		/// <summary>
		/// Unsets (clears) previously set references to dependent components.
		/// </summary>
		public virtual void UnsetReferences()
		{
			// Override in child class
		}

		protected virtual void InitReferences(IReferences references)
		{
			var existingInfo = references.GetOneOptional<ContextInfo>(DefaultInfoFactory.ContextInfoDescriptor);
			if (existingInfo == null)
				references.Put(DefaultInfoFactory.ContextInfoDescriptor, _info);
			else _info = existingInfo;

			references.Put(DefaultContainerFactory.Descriptor, _factories);
		}

		/// <summary>
		/// Adds a factory to the container. The factory is used to create components
		/// added to the container by their locators(descriptors).
		/// </summary>
		/// <param name="factory">a component factory to be added.</param>
		public void AddFactory(IFactory factory)
		{
			_factories.Add(factory);
		}

		/// <summary>
		/// Checks if the component is opened.
		/// </summary>
		/// <returns>true if the component has been opened and false otherwise.</returns>
		public virtual bool IsOpen()
		{
			return _references != null;
		}

		/// <summary>
		/// Opens the component.
		/// </summary>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		/// <returns></returns>
		public async Task OpenAsync(string correlationId)
		{
			if (_references != null)
				throw new InvalidStateException(correlationId, "ALREADY_OPENED", "Container was already opened");

			//if (_config == null)
			//    throw new InvalidStateException(correlationId, "NO_CONFIG", "Container was not configured");

			try
			{
				_logger.Trace(correlationId, "Starting container.");

				// Create references with configured components
				_references = new ContainerReferences();
				InitReferences(_references);
				_references.PutFromConfig(_config);
				SetReferences(_references);

				// Get custom description if available
				var infoDescriptor = new Descriptor("*", "context-info", "*", "*", "*");
				_info = _references.GetOneRequired<ContextInfo>(infoDescriptor);

				await _references.OpenAsync(correlationId);

				// Get reference to logger
				_logger = new CompositeLogger(_references);
				_logger.Info(correlationId, "Container {0} started.", _info.Name);
			}
			catch (Exception ex)
			{
				_logger.Error(correlationId, ex, "Failed to start container");

				await CloseAsync(correlationId);

				throw;
			}
		}

		/// <summary>
		/// Closes component and frees used resources.
		/// </summary>
		/// <param name="correlationId">(optional) transaction id to trace execution through call chain.</param>
		/// <returns></returns>
		public async Task CloseAsync(string correlationId)
		{
			if (_references == null)
				return;

			try
			{
				_logger.Trace(correlationId, "Stopping {0} container", _info.Name);

				// Close and dereference components
				await _references.CloseAsync(correlationId);
				_references = null;

				_logger.Info(correlationId, "Container {0} stopped", _info.Name);
			}
			catch (Exception ex)
			{
				_logger.Error(correlationId, ex, "Failed to stop container");
				throw;
			}
		}
	}
}
