using PipServices4.Components.Build;
using PipServices4.Components.Refer;
using PipServices4.Mysql.Connect;

namespace PipServices4.Mysql.Build
{
    /// <summary>
    /// Creates MySql components by their descriptors.
    /// </summary>
    /// See <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/class_pip_services_1_1_components_1_1_build_1_1_factory.html">Factory</a>, 
    /// <a href="https://pip-services4-dotnet.github.io/pip-services4-mysql-dotnet/class_pip_services3_1_1_sql_server_1_1_persistence_1_1_sql_server_connection.html">MySqlConnection</a>
    public class DefaultMySqlFactory : Factory
    {
        public static Descriptor Descriptor = new Descriptor("pip-services", "factory", "mysql", "default", "1.0");
        public static Descriptor MySqlConnectionDescriptor = new Descriptor("pip-services", "connection", "mysql", "*", "1.0");

        /// <summary>
        /// Create a new instance of the factory.
        /// </summary>
        public DefaultMySqlFactory()
        {
            RegisterAsType(MySqlConnectionDescriptor, typeof(MySqlConnection));
        }
    }
}
