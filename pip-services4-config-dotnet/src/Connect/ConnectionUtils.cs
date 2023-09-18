using PipServices4.Commons.Data;
using PipServices4.Components.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipServices4.Config.Connect
{
	/// <summary>
	/// A set of utility functions to process connection parameters
	/// </summary>
	public class ConnectionUtils
    {
        /// <summary>
        /// Concatinates two options by combining duplicated properties into comma-separated list
        /// </summary>
        /// <param name="options1">first options to merge</param>
        /// <param name="options2">second options to merge</param>
        /// <param name="keys">when define it limits only to specific keys</param>
        public static ConfigParams Concat(ConfigParams options1, ConfigParams options2, params string[] keys)
        {
            var options = ConfigParams.FromValue(options1);
            foreach (var key in options2.Keys)
            {
                var value1 = options1.GetAsString(key) != null ? options1.GetAsString(key) : "" ;
                var value2 = options2.GetAsString(key) != null ? options2.GetAsString(key) : "";

                if (value1 != "" && value2 != "")
                {
                    if (keys == null || keys.Length == 0 || ((IList<string>)keys).Contains(key))
                    {
                        options.SetAsObject(key, value1 + "," + value2);
                    }
                }
                else if (value1 != "")
                {
                    options.SetAsObject(key, value1);
                }
                else if (value2 != "")
                {
                    options.SetAsObject(key, value2);
                }
            }
            return options;
        }

        private static string ConcatValues(string value1, string value2)
        {
            if (value1 == null || value1 == "") return value2;
            if (value2 == null || value2 == "") return value1;
            return value1 + "," + value2;
        }

        /// <summary>
        /// Parses URI into config parameters.
        /// The URI shall be in the following form:
        ///     `protocol://username@password@host1:port1,host2:port2,...?param1=abc&param2=xyz&...`
        /// </summary>
        /// <param name="uri">the URI to be parsed</param>
        /// <param name="defaultProtocol">a default protocol</param>
        /// <param name="defaultPort">a default port</param>
        /// <returns>a configuration parameters with URI elements</returns>
        public static ConfigParams ParseUri(string uri, string defaultProtocol, int defaultPort)
        {
            var options = new ConfigParams();

            if (uri == null || uri == "") return options;

            uri = uri.Trim();

            // Process parameters
            int pos = uri.IndexOf("?");
            if (pos > 0)
            {
                var parameters = uri.Substring(pos + 1);
                uri = uri.Substring(0, pos);

                var paramsList = parameters.Split('&');
                foreach (var param in paramsList)
                {
                    pos = param.IndexOf("=");
                    if (pos >= 0)
                    {
                        var key = Uri.UnescapeDataString(param.Substring(0, pos));
                        var value = Uri.UnescapeDataString(param.Substring(pos + 1));
                        options.SetAsObject(key, value);
                    }
                    else
                    {
                        options.SetAsObject(Uri.UnescapeDataString(param), null);
                    }
                }
            }

            // Process protocol
            pos = uri.IndexOf("://");
            if (pos > 0)
            {
                var protocol = uri.Substring(0, pos);
                uri = uri.Substring(pos + 3);
                options.SetAsObject("protocol", protocol);
            }
            else
            {
                options.SetAsObject("protocol", defaultProtocol);
            }

            // Process user and password
            pos = uri.IndexOf("@");
            if (pos > 0)
            {
                var userAndPass = uri.Substring(0, pos);
                uri = uri.Substring(pos + 1);

                pos = userAndPass.IndexOf(":");
                if (pos > 0)
                {
                    options.SetAsObject("username", userAndPass.Substring(0, pos));
                    options.SetAsObject("password", userAndPass.Substring(pos + 1));
                }
                else
                {
                    options.SetAsObject("username", userAndPass);
                }
            }

            // Process host and ports
            var servers = uri.Split(',');
            foreach (var server in servers)
            {
                pos = server.IndexOf(":");
                if (pos > 0)
                {
                    options.SetAsObject("servers", ConnectionUtils.ConcatValues(options.GetAsString("servers"), server));
                    options.SetAsObject("host", ConnectionUtils.ConcatValues(options.GetAsString("host"), server.Substring(0, pos)));
                    options.SetAsObject("port", ConnectionUtils.ConcatValues(options.GetAsString("port"), server.Substring(pos + 1)));
                }
                else
                {
                    options.SetAsObject("servers", ConnectionUtils.ConcatValues(options.GetAsString("servers"), server + ":" + defaultPort.ToString()));
                    options.SetAsObject("host", ConnectionUtils.ConcatValues(options.GetAsString("host"), server));
                    options.SetAsObject("port", ConnectionUtils.ConcatValues(options.GetAsString("port"), defaultPort.ToString()));
                }
            }

            return options;
        }

        /// <summary>
        /// Composes URI from config parameters.
        /// The result URI will be in the following form:
        ///     `protocol://username@password@host1:port1,host2:port2,...?param1=abc&param2=xyz&...`
        /// </summary>
        /// <param name="options">configuration parameters</param>
        /// <param name="defaultProtocol">a default protocol</param>
        /// <param name="defaultPort">a default port</param>
        /// <returns>a composed URI</returns>
        public static string ComposeUri(ConfigParams options, string defaultProtocol, int defaultPort)
        {
            StringBuilder builder = new StringBuilder();

            var protocol = options.GetAsStringWithDefault("protocol", defaultProtocol);
            if (protocol != null)
                builder.Append(protocol + "://" + builder);

            var username = options.GetAsNullableString("username");
            if (username != null)
            {
                builder.Append(username);
                var password = options.GetAsNullableString("password");
                if (password != null)
                    builder.Append(":" + password);

                builder.Append("@");
            }

            StringBuilder servers = new StringBuilder();
            var defaultPortStr = defaultPort > 0 ? defaultPort.ToString() : "";
            var hosts = options.GetAsStringWithDefault("host", "???").Split(',');
            var ports = options.GetAsStringWithDefault("port", defaultPortStr).Split(',');
            for (int index = 0; index < hosts.Length; index++)
            {
                if (servers.Length > 0)
                    servers.Append(',');

                var host = hosts[index];
                servers.Append(host);

                var port = ports.Length > index ? ports[index] : defaultPortStr;
                port = port != "" ? port : defaultPortStr;

                if (port != "")
                    servers.Append(":" + port);
            }
            builder.Append(servers);

            var parameters = new StringBuilder();
            List<string> reservedKeys = new List<string> { "protocol", "host", "port", "username", "password", "servers" };
            foreach (var key in options.Keys)
            {
                if (reservedKeys.Contains(key))
                    continue;

                if (parameters.Length > 0)
                    parameters.Append("&");

                parameters.Append(Uri.EscapeDataString(key));

                var value = options.GetAsNullableString(key);
                if (value != null && value != "")
                    parameters.Append("=" + Uri.EscapeDataString(value));
            }

            if (parameters.Length > 0)
                builder.Append("?" + parameters);

            return builder.ToString();

        }

        /// <summary>
        /// Includes specified keys from the config parameters.
        /// </summary>
        /// <param name="options">configuration parameters to be processed.</param>
        /// <param name="keys">a list of keys to be included.</param>
        /// <returns>a processed config parameters.</returns>
        public static ConfigParams Include(ConfigParams options, params string[] keys)
        {
            if (keys == null || keys.Length == 0) return options;

            var result = new ConfigParams();

            foreach (var key in options.Keys)
            {
                if (((IList<string>)keys).Contains(key))
                    result.SetAsObject(key, options.GetAsString(key));
            }

            return result;
        }

        /// <summary>
        /// Excludes specified keys from the config parameters.
        /// </summary>
        /// <param name="options">configuration parameters to be processed.</param>
        /// <param name="keys">a list of keys to be excluded.</param>
        /// <returns>a processed config parameters.</returns>
        public static ConfigParams Exclude(ConfigParams options, params string[] keys)
        {
            if (keys == null || keys.Length == 0) return options;

            ConfigParams result = new ConfigParams((StringValueMap)options.Clone());

            foreach (var key in keys)
                result.Remove(key);

            return result;
        }
    }
}