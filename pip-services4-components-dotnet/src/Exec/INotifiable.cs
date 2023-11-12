using PipServices4.Components.Context;
using System.Threading.Tasks;

namespace PipServices4.Components.Exec
{
    /// <summary>
    /// Interface for components that can be asynchronously notified.
    /// The notification may include optional argument that describe the occured event.
    /// </summary>
    /// <example>
    /// <code>
    /// class MyComponent: INotifable 
    /// {
    ///     ...
    ///     public void Notify(IContext context, Parameters args)
    ///     {
    ///         Console.WriteLine("Occured event " + args.GetAsString("event"));
    ///     }
    /// }
    /// 
    /// var myComponent = new MyComponent();
    /// myComponent.Notify("123", Parameters.FromTuples("event", "Test Event"));
    /// </code>
    /// </example>
    /// See <see cref="Notifier"/>, <see cref="IExecutable"/>
    public interface INotifiable
    {
        /// <summary>
        /// Notifies the component about occured event.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="args">notification arguments.</param>
        Task NotifyAsync(IContext context, Parameters args);
    }
}
