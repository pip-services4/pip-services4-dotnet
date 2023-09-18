using PipServices4.Commons.Convert;

namespace PipServices4.Data.Validate
{
    public class SortParamsSchema : ArraySchema
    {
        public SortParamsSchema()
            : base(new SortFieldSchema())
        {
        }
    }
}