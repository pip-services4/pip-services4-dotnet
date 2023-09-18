using PipServices4.Commons.Convert;

namespace PipServices4.Data.Validate
{
    /// <summary>
    /// Schema to validate FilterParams.
    /// </summary>
    /// See <see cref="FilterParams"/>
    public class FilterParamsSchema : MapSchema
    {
        /// <summary>
        /// Creates a new instance of validation schema.
        /// </summary>
        public FilterParamsSchema()
            : base(TypeCode.String, null)
        {
        }
    }
}