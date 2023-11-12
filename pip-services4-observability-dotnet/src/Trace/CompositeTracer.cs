using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using System;
using System.Collections.Generic;

namespace PipServices4.Observability.Trace
{
	/// <summary>
	/// Aggregates all tracers from component references under a single component.
	/// It allows to record traces and conveniently send them to multiple destinations. 
	/// 
	/// ### References ###
	/// 
	/// - *:tracer:*:*:1.0    (optional) <see cref="ITracer"/> components to pass operation traces
	/// 
	/// See <see cref="ITracer"/>
	/// 
	/// <example>
	/// <code>
	/// class MyComponent: IReferenceable {
	///     private CompositeTracer _tracer = new CompositeTracer();
	/// 
	///     public void SetReferences(IReferences references)
	///     {
	///         _tracer.SetReferences(references);
	///         ...
	///     }
	/// 
	///     public void MyMethod(string correlatonId)
	///     {
	///         var timing = this._tracer.BeginTrace(context, "mycomponent", "mymethod");
	///         try {
	///             ...
	///             timing.EndTrace();
	///         } catch {
	///             timing.EndFailure(err);
	///         }
	///     }
	/// }
	/// </code>
	/// </example>
	/// </summary>
	public class CompositeTracer : ITracer, IReferenceable
	{
		protected readonly IList<ITracer> _tracers = new List<ITracer>();

		/// <summary>
		/// Creates a new instance of the tracer.
		/// </summary>
		/// <param name="references">references to locate the component dependencies. </param>
		public CompositeTracer(IReferences references = null)
		{
			if (references != null)
				SetReferences(references);
		}

		/// <summary>
		/// Sets references to dependent components.
		/// </summary>
		/// <param name="references">references to locate the component dependencies. </param>
		public void SetReferences(IReferences references)
		{
			var tracers = references.GetOptional<ITracer>(new Descriptor(null, "tracer", null, null, null));
			for (int i = 0; i < tracers.Count; i++)
			{
				var tracer = tracers[i];
				if (tracer != this)
					_tracers.Add(tracer);
			}
		}

		/// <summary>
		/// Records an operation trace with its name and duration
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="component">a name of called component</param>
		/// <param name="operation">a name of the executed operation. </param>
		/// <param name="duration">execution duration in milliseconds. </param>
		public void Trace(IContext context, string component, string operation, long duration)
		{
			foreach (var tracer in _tracers)
				tracer.Trace(context, component, operation, duration);
		}

		/// <summary>
		/// Records an operation failure with its name, duration and error
		/// </summary>
		/// <param name="context"> (optional) transaction id to trace execution through call chain.</param>
		/// <param name="component">a name of called component</param>
		/// <param name="operation">a name of the executed operation. </param>
		/// <param name="error">an error object associated with this trace.</param>
		/// <param name="duration">execution duration in milliseconds. </param>
		public void Failure(IContext context, string component, string operation, Exception error, long duration)
		{
			foreach (var tracer in _tracers)
				tracer.Failure(context, component, operation, error, duration);
		}

		/// <summary>
		/// Begings recording an operation trace
		/// </summary>
		/// <param name="context">(optional) transaction id to trace execution through call chain.</param>
		/// <param name="component">a name of called component</param>
		/// <param name="operation">a name of the executed operation. </param>
		/// <returns>a trace timing object.</returns>
		public TraceTiming BeginTrace(IContext context, string component, string operation)
		{
			return new TraceTiming(context, component, operation, this);
		}
	}
}