using PipServices4.Commons.Data;
using PipServices4.Components.Config;
using System.Collections.Generic;

namespace PipServices4.Config.Auth
{
	/// <summary>
	/// Contains credentials to authenticate against external services.
	/// They are used together with connection parameters, but usually stored
	/// in a separate store, protected from unauthorized access.
	/// 
	/// ### Configuration parameters ###
	/// 
	/// - store_key:     key to retrieve parameters from credential store
	/// - username:      user name
	/// - user:          alternative to username
	/// - password:      user password
	/// - pass:          alternative to password
	/// - access_id:     application access id
	/// - client_id:     alternative to access_id
	/// - access_key:    application secret key
	/// - client_key:    alternative to access_key
	/// - secret_key:    alternative to access_key
	/// 
	/// In addition to standard parameters CredentialParams may contain any number of custom parameters
	/// </summary>
	/// <example>
	/// <code>
	/// var credential = CredentialParams.FromTuples(
	/// "user", "jdoe",
	/// "pass", "pass123",
	/// "pin", "321" );
	/// 
	/// var username = credential.GetUsername();             // Result: "jdoe"
	/// var password = credential.GetPassword();             // Result: "pass123"
	/// var pin = credential.GetAsNullableString("pin");     // Result: 321 
	/// </code>
	/// </example>
	/// See <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/class_pip_services3_1_1_commons_1_1_config_1_1_config_params.html">ConfigParams</a>, 
	/// <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/class_pip_services_1_1_components_1_1_connect_1_1_connection_params.html"/>ConnectionParams</a>, 
	/// <see cref="CredentialResolver"/>, <see cref="ICredentialStore"/>
	public class CredentialParams : ConfigParams
    {
        /// <summary>
        /// Creates an empty instance of credential parameters.
        /// </summary>
        public CredentialParams() { }

        /// <summary>
        /// Creates a new ConfigParams and fills it with values.
        /// </summary>
        /// <param name="map">(optional) an object to be converted into key-value pairs to 
        /// initialize these credentials.</param>
        public CredentialParams(IDictionary<string, string> map)
            : base(map)
        { }

        /// <summary>
        /// Checks if these credential parameters shall be retrieved from
        /// CredentialStore.The credential parameters are redirected to CredentialStore
        /// when store_key parameter is set.
        /// </summary>
        /// <returns>true if credentials shall be retrieved from CredentialStore</returns>
        public bool UseCredentialStore
        {
            get { return ContainsKey("store_key"); }
        }

        /// <summary>
        /// Gets or setsthe key to retrieve these credentials from CredentialStore. 
        /// </summary>
        public string StoreKey
        {
            get { return GetAsNullableString("store_key"); }
            set { this["store_key"] = value; }
        }

        /// <summary>
        /// Gets or sets the user name / login.
        /// </summary>
        public string Username
        {
            get { return GetAsNullableString("username"); }
            set { this["username"] = value; }
        }

        /// <summary>
        /// Gets or sets the service user password.
        /// </summary>
        public string Password
        {
            get { return GetAsNullableString("password"); }
            set { this["password"] = value; }
        }

        /// <summary>
        /// Gets or sets the client or access id
        /// </summary>
        public string AccessId
        {
            get
            {
                string accessId = GetAsNullableString("access_id");
                accessId = accessId ?? GetAsNullableString("client_id");
                return accessId;
            }
            set { this["access_id"] = value; }
        }

        /// <summary>
        /// Gets or sets the client or access key
        /// </summary>
        public string AccessKey
        {
            get
            {
                var accessKey = GetAsNullableString("access_key");
                accessKey = accessKey ?? GetAsNullableString("client_key");
                accessKey = accessKey ?? GetAsNullableString("secret_key");
                return accessKey;
            }
            set { this["access_key"] = value; }
        }

        /// <summary>
        /// Creates a new CredentialParams object filled with key-value pairs serialized as a string.
        /// </summary>
        /// <param name="line">a string with serialized key-value pairs as
        /// "key1=value1;key2=value2;..." Example:
        /// "Key1=123;Key2=ABC;Key3=2016-09-16T00:00:00.00Z"</param>
        /// <returns>a new CredentialParams object.</returns>
        public new static CredentialParams FromString(string line)
        {
            var map = StringValueMap.FromString(line);
            return new CredentialParams(map);
        }

        /// <summary>
        /// Retrieves all CredentialParams from configuration parameters from
        /// "credentials" section.If "credential" section is present instead, than it
        /// returns a list with only one CredentialParams.
        /// </summary>
        /// <param name="config">a configuration parameters to retrieve credentials</param>
        /// <param name="configAsDefault">boolean parameter for default configuration. If "true"
        /// the default value will be added to the result.</param>
        /// <returns>a list of retrieved CredentialParams</returns>
        public static List<CredentialParams> ManyFromConfig(ConfigParams config, bool configAsDefault = true)
        {
            var result = new List<CredentialParams>();

            // Try to get multiple credentials first
            var credentials = config.GetSection("credentials");

            if (credentials.Count > 0)
            {
                var sectionsNames = credentials.GetSectionNames();

                foreach (var section in sectionsNames)
                {
                    var credential = credentials.GetSection(section);
                    result.Add(new CredentialParams(credential));
                }
            }
            // Then try to get a single connection
            else
            {
                var credential = config.GetSection("credential");
                if (credential.Count > 0)
                    result.Add(new CredentialParams(credential));
                // Apply defaults
                else if (configAsDefault)
                    result.Add(new CredentialParams(config));
            }

            return result;
        }

        /// <summary>
        /// Retrieves a single CredentialParams from configuration parameters from
        /// "credential" section.If "credentials" section is present instead, then is
        /// returns only the first credential element.
        /// </summary>
        /// <param name="config">ConfigParams, containing a section named "credential(s)".</param>
        /// <param name="configAsDefault">boolean parameter for default configuration. If "true"
        /// the default value will be added to the result.</param>
        /// <returns>the generated CredentialParams object.</returns>
        public static CredentialParams FromConfig(ConfigParams config, bool configAsDefault = true)
        {
            var connections = ManyFromConfig(config, configAsDefault);
            return connections.Count > 0 ? connections[0] : null;
        }
    }
}
