using TypeCode = PipServices4.Commons.Convert.TypeCode;

namespace PipServices4.Data.Validate
{
	/// <summary>
	/// Schema to validate PagingParams.
	/// </summary>
	/// See <see cref="PagingParams"/>
	public class PagingParamsSchema : ObjectSchema
    {
        /// <summary>
        /// Creates a new instance of validation schema.
        /// </summary>
        public PagingParamsSchema()
        {
            WithOptionalProperty("skip", typeof(long));
            WithOptionalProperty("take", typeof(long));
            WithOptionalProperty("total", TypeCode.Boolean);
        }
    }
}