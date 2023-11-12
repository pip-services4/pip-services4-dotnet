using PipServices4.Components.Context;
using System;

namespace PipServices4.Observability.Trace
{
	/// <summary>
	/// Timing object returned by {@link ITracer.beginTrace} to end timing
	/// of execution block and record the associated trace.
	/// 
	/// <example>
	/// <code>
	/// var timing = tracer.BeginTrace("mymethod.exec_time");
	/// try 
	/// {
	///     ...
	///     timing.EndTrace();
	/// } 
	/// catch (Exception err)
	/// {
	///      timing.EndFailure(err);
	/// }
	/// </code>
	/// </example>
	/// </summary>
	public class TraceTiming
	{
		private long _start;
		private ITracer _tracer;
		private IContext _context;
		private string _component;
		private string _operation;

		/// <summary>
		/// Creates a new instance of the timing callback object.
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="component">an associated component name</param>
		/// <param name="operation">an associated operation name</param>
		/// <param name="tracer">a callback that shall be called when endTiming is called.</param>
		public TraceTiming(IContext context, string component, string operation, ITracer tracer = null)
		{
			_context = context;
			_component = component;
			_operation = operation;
			_tracer = tracer;
			_start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		}

		/// <summary>
		/// Ends timing of an execution block, calculates elapsed time
		/// and records the associated trace.
		/// </summary>
		public void EndTrace()
		{
			if (_tracer != null)
			{
				long elapsed = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _start;
				_tracer.Trace(_context, _component, _operation, elapsed);
			}
		}

		/// <summary>
		/// Ends timing of a failed block, calculates elapsed time
		/// and records the associated trace.
		/// </summary>
		/// <param name="error">an error object associated with this trace.</param>
		public void EndFailure(Exception error)
		{
			if (_tracer != null)
			{
				long elapsed = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _start;
				_tracer.Failure(_context, _component, _operation, error, elapsed);
			}
		}
	}
}
