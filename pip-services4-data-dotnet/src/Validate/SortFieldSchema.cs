using PipServices4.Commons.Convert;

namespace PipServices4.Data.Validate
{
    public class SortFieldSchema : ObjectSchema
    {
        public SortFieldSchema()
        {
            WithOptionalProperty("name", TypeCode.String);
            WithOptionalProperty("ascending", TypeCode.Boolean);
        }
    }
}