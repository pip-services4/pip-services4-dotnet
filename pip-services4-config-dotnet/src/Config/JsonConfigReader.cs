using PipServices4.Commons.Convert;
using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using System;
using System.IO;

namespace PipServices4.Config.Config
{
	/// <summary>
	/// Config reader that reads configuration from JSON file.
	/// 
	/// The reader supports parameterization using Handlebars template engine.
	/// 
	/// ### Configuration parameters ###
	/// 
	/// - path:          path to configuration file
	/// - parameters:    this entire section is used as template parameters
	/// - ...
	/// </summary>
	/// <example>
	/// <code>
	/// ======== config.json ======
	/// { "key1": "{{KEY1_VALUE}}", "key2": "{{KEY2_VALUE}}" }
	/// ===========================
	/// 
	/// var configReader = new JsonConfigReader("config.json");
	/// var parameters = ConfigParams.FromTuples("KEY1_VALUE", 123, "KEY2_VALUE", "ABC");
	/// configReader.ReadConfig("123", parameters);
	/// </code>
	/// </example>
	/// See <see cref="IConfigReader"/>, <see cref="FileConfigReader"/>
	public class JsonConfigReader : FileConfigReader
    {
        /// <summary>
        /// Creates a new instance of the config reader.
        /// </summary>
        /// <param name="path">(optional) a path to configuration file.</param>
        public JsonConfigReader(string path = null)
            : base(path)
        { }

        /// <summary>
        /// Reads configuration file, parameterizes its content and converts it into JSON object.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="parameters">values to parameters the configuration.</param>
        /// <returns>a JSON object with configuration.</returns>
        private object ReadObject(IContext context, ConfigParams parameters)
        {
            var traceId = context != null ? ContextResolver.GetTraceId(context) : null;

            if (Path == null)
            {
                throw new ConfigException(traceId, "NO_PATH", "Missing config file path");
            }

            try
            {
                using (var reader = new StreamReader(File.OpenRead(Path)))
                {
                    var json = reader.ReadToEnd();
                    json = Parameterize(json, parameters);
                    return JsonConverter.ToNullableMap(json);
                }
            }
            catch (Exception ex)
            {
                throw new FileException(
                    traceId,
                    "READ_FAILED",
                    "Failed reading configuration " + Path + ": " + ex
                )
                .WithDetails("path", Path)
                .WithCause(ex);
            }
        }

        /// <summary>
        /// Reads configuration and parameterize it with given values.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="parameters">values to parameters the configuration</param>
        /// <returns>ConfigParams configuration.</returns>
        public override ConfigParams ReadConfig(IContext context, ConfigParams parameters)
        {
            var value = ReadObject(context, parameters);
            return ConfigParams.FromValue(value);
        }

        /// <summary>
        /// Reads configuration file, parameterizes its content and converts it into JSON object.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="path">a path to configuration file.</param>
        /// <param name="parameters">values to parameters the configuration.</param>
        /// <returns>a JSON object with configuration.</returns>
        public static object ReadObject(IContext context, string path, ConfigParams parameters)
        {
            return new JsonConfigReader(path).ReadObject(context, parameters);
        }

        /// <summary>
        /// Reads configuration from a file, parameterize it with given values 
        /// and returns a new ConfigParams object.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="path">a path to configuration file.</param>
        /// <param name="parameters">values to parameters the configuration</param>
        /// <returns>ConfigParams configuration.</returns>
        public static ConfigParams ReadConfig(IContext context, string path, ConfigParams parameters)
        {
            return new JsonConfigReader(path).ReadConfig(context, parameters);
        }
    }
}
