using PipServices4.Components.Build;
using PipServices4.Components.Refer;
using PipServices4.Sqlserver.Connect;

namespace PipServices4.Sqlserver.Build
{
    /// <summary>
    /// Creates SqlServer components by their descriptors.
    /// </summary>
    /// See <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/class_pip_services_1_1_components_1_1_build_1_1_factory.html">Factory</a>, 
    /// <a href="https://pip-services4-dotnet.github.io/pip-services4-sqlserver-dotnet/class_pip_services3_1_1_sql_server_1_1_persistence_1_1_sql_server_connection.html">SqlServerConnection</a>
    public class DefaultSqlServerFactory : Factory
    {
        public static Descriptor Descriptor = new Descriptor("pip-services", "factory", "sqlserver", "default", "1.0");
        public static Descriptor SqlServerConnectionDescriptor = new Descriptor("pip-services", "connection", "sqlserver", "*", "1.0");

        /// <summary>
        /// Create a new instance of the factory.
        /// </summary>
        public DefaultSqlServerFactory()
        {
            RegisterAsType(SqlServerConnectionDescriptor, typeof(SqlServerConnection));
        }
    }
}
