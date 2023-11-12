using PipServices4.Components.Context;
using PipServices4.Components.Exec;

namespace PipServices4.Rpc.Commands
{
    /// <summary>
    /// An interface for listener objects that receive notifications on fired events.
    /// </summary>
    /// <example>
    /// <code>
    /// public class MyListener: IEventListener {
    ///     private Task onEvent(IContext context, IEvent event, Parameters args)  {
    ///         Console.WriteLine("Fired event " + event.getName());
    ///     }}
    ///     
    /// Event event = new Event("myevent");
    /// event.addListener(new MyListener()); 
    /// event.notify("123", Parameters.fromTuples("param1", "ABC")); 
    /// // Console output: Fired event myevent
    /// </code>
    /// </example>
    /// See <see cref="IEvent"/>, <see cref="Event"/>
    public interface IEventListener
    {
        /// <summary>
        /// A method called when events this listener is subscrubed to are fired.
        /// </summary>
        /// <param name="e">a fired event</param>
        /// <param name="context">optional transaction id to trace calls across components.</param>
        /// <param name="value">Event arguments/value.</param>
        void OnEvent(IContext context, IEvent e, Parameters value);
    }
}