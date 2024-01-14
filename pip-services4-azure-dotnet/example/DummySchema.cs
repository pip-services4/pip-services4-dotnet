using PipServices4.Data.Validate;
using System;

namespace PipServices4.Azure
{
    public class DummySchema: ObjectSchema
    {
        public DummySchema()
        {
            WithOptionalProperty("id", TypeCode.String);
            WithRequiredProperty("key", TypeCode.String);
            WithOptionalProperty("content", TypeCode.String);
        }
    }
}