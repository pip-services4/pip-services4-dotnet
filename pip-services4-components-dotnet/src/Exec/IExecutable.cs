using PipServices4.Components.Context;
using System.Threading.Tasks;

namespace PipServices4.Components.Exec
{
    /// <summary>
    /// Interface for components that can be called to execute work.
    /// </summary>
    /// <example>
    /// <code>
    /// class EchoComponent: IExecutable 
    /// {
    ///     ...
    ///     public void Execute(IContext context, Parameters args)
    ///     {
    ///         var result = args.GetAsObject("message");
    ///     }
    /// }
    /// 
    /// var echo = new EchoComponent();
    /// string message = "Test";
    /// echo.Execute("123", Parameters.FromTuples("message", message));
    /// </code>
    /// </example>
    /// See <see cref="Executor"/>, <see cref="INotifiable"/>, <see cref="Parameters"/>
    public interface IExecutable
    {
        /// <summary>
        /// Executes component with arguments and receives execution result.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        /// <param name="args">execution arguments.</param>
        /// <returns>execution result</returns>
        Task<object> ExecuteAsync(IContext context, Parameters args);
    }
}
