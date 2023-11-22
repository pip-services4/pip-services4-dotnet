using PipServices4.Components.Build;
using PipServices4.Components.Refer;
using PipServices4.Mongodb.Connect;

namespace PipServices4.MongoDb.Build
{
    /// <summary>
    /// Creates MongoDB components by their descriptors.
    /// </summary>
    /// See <a href="https://pip-services3-dotnet.github.io/pip-services3-components-dotnet/class_pip_services_1_1_components_1_1_build_1_1_factory.html">Factory</a>, 
    /// <a href="https://pip-services3-dotnet.github.io/pip-services3-mongodb-dotnet/class_pip_services3_1_1_mongo_db_1_1_persistence_1_1_mongo_db_connection.html">MongoDbConnection</a>
    public class DefaultMongoDbFactory : Factory
    {
        public static Descriptor Descriptor = new Descriptor("pip-services", "factory", "mongodb", "default", "1.0");
        public static Descriptor MongoDbConnectionDescriptor = new Descriptor("pip-services", "connection", "mongodb", "*", "1.0");

        /// <summary>
        /// Create a new instance of the factory.
        /// </summary>
        public DefaultMongoDbFactory()
        {
            RegisterAsType(MongoDbConnectionDescriptor, typeof(MongoDbConnection));
        }
    }
}
