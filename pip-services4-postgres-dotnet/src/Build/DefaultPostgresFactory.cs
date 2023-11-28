using PipServices4.Components.Build;
using PipServices4.Components.Refer;
using PipServices4.Postgres.Connect;

namespace PipServices4.Postgres.Build
{
    /// <summary>
    /// Creates PostgreSQL components by their descriptors.
    /// </summary>
    /// See <a href="https://pip-services4-dotnet.github.io/pip-services4-components-dotnet/class_pip_services_1_1_components_1_1_build_1_1_factory.html">Factory</a>, 
    /// <a href="https://pip-services4-dotnet.github.io/pip-services4-postgres-dotnet/class_pip_services3_1_1_postgres_1_1_persistence_1_1_postgres_connection.html">PostgresConnection</a>
    public class DefaultPostgresFactory : Factory
    {
        public static Descriptor Descriptor = new Descriptor("pip-services", "factory", "postgres", "default", "1.0");
        public static Descriptor PostgresConnectionDescriptor = new Descriptor("pip-services", "connection", "postgres", "*", "1.0");

        /// <summary>
        /// Create a new instance of the factory.
        /// </summary>
        public DefaultPostgresFactory()
        {
            RegisterAsType(PostgresConnectionDescriptor, typeof(PostgresConnection));
        }
    }
}
