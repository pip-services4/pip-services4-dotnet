using PipServices4.Commons.Convert;
using PipServices4.Data.Validate;

namespace PipServices4.Swagger
{
    public class DummySchema : ObjectSchema
    {
        public DummySchema()
        {
            WithOptionalProperty("id", TypeCode.String);
            WithRequiredProperty("key", TypeCode.String);
            WithOptionalProperty("content", TypeCode.String);
            WithOptionalProperty("flag", TypeCode.Boolean);
            WithOptionalProperty("param", new DummyParamSchema());
            WithOptionalProperty("items", new ArraySchema(new DummyItemSchema()));
            WithOptionalProperty("tags", new ArraySchema(TypeCode.String));
            WithOptionalProperty("date", TypeCode.DateTime);
        }
    }

    public class DummyParamSchema : ObjectSchema
    {
        public DummyParamSchema()
        {
            WithOptionalProperty("name", TypeCode.String);
            WithRequiredProperty("value", TypeCode.Double);
        }
    }

    public class DummyItemSchema : ObjectSchema
    {
        public DummyItemSchema()
        {
            WithOptionalProperty("name", TypeCode.String);
            WithRequiredProperty("count", TypeCode.Integer);
        }
    }
}
