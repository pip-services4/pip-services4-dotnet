namespace PipServices4.Components.Context
{
	/// <summary>
	/// Context resolver that processes context and extracts values from there.
	/// </summary>
	/// <see cref="Context"/>
	public class ContextResolver
	{
		/// <summary>
		/// Extracts trace id from execution context. 
		/// </summary>
		/// <param name="context">execution context to trace execution through call chain.</param>
		/// <returns>a trace id or <code>null</code> if it is not defined.</returns>
		/// <see cref="Context"/>
		public static string GetTraceId(IContext context)
		{
			var traceId = context.Get("trace_id") ?? context.Get("traceId");
			return traceId?.ToString();
		}

		/// <summary>
		/// Extracts client name from execution context. 
		/// </summary>
		/// <param name="context">execution context to trace execution through call chain.</param>
		/// <returns>a client name or <code>null</code> if it is not defined.</returns>
		/// <see cref="Context"/>
		public static string GetClient(IContext context)
		{
			var client = context.Get("client");
			return client?.ToString();
		}

		/// <summary>
		/// Extracts user name (identifier) from execution context. 
		/// </summary>
		/// <param name="context">execution context to trace execution through call chain.</param>
		/// <returns>a user reference or <code>null</code> if it is not defined.</returns>
		/// <see cref="Context"/>
		public static string GetUser(IContext context)
		{
			var client = context.Get("user");
			return client?.ToString();
		}
	}
}
