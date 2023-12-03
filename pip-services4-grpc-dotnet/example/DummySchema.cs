using PipServices4.Commons.Convert;
using PipServices4.Data.Validate;

namespace PipServices4.Grpc
{
    public class DummySchema : ObjectSchema
    {
        public DummySchema()
        {
            WithOptionalProperty("id", TypeCode.String);
            WithRequiredProperty("key", TypeCode.String);
            WithOptionalProperty("content", TypeCode.String);
        }
    }
}
