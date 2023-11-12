using PipServices4.Components.Context;
using PipServices4.Components.Exec;
using PipServices4.Data.Validate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Rpc.Commands
{
    /// <summary>
    /// An interface for stackable command interceptors, which can extend
    /// and modify the command call chain.
    /// 
    /// This mechanism can be used for authentication, logging, and other functions.
    /// </summary>
    /// See <see cref="ICommand"/>, <see cref="InterceptedCommand"/>
    public interface ICommandInterceptor
    {
        /// <summary>
        /// Gets the name of the wrapped command.
        /// 
        /// The interceptor can use this method to override the command name.
        /// Otherwise it shall just delegate the call to the wrapped command.
        /// </summary>
        /// <param name="command">the next command in the call chain.</param>
        /// <returns>the name of the wrapped command.</returns>
        string GetName(ICommand command);

        /// <summary>
        /// Executes the wrapped command with specified arguments.
        /// 
        /// The interceptor can use this method to intercept and alter the command
        /// execution.Otherwise it shall just delete the call to the wrapped command.
        /// </summary>
        /// <param name="context">optional transaction id to trace calls across components.</param>
        /// <param name="command">the next command in the call chain that is to be executed.</param>
        /// <param name="args">the parameters (arguments) to pass to the command for execution.</param>
        /// <returns>execution result.</returns>
        /// See <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/class_pip_services3_1_1_commons_1_1_run_1_1_parameters.html"/>Parameters</a>
        Task<object> ExecuteAsync(IContext context, ICommand command, Parameters args);

        /// <summary>
        /// Validates arguments of the wrapped command before its execution.
        /// 
        /// The interceptor can use this method to intercept and alter validation of the
        /// command arguments.Otherwise it shall just delegate the call to the wrapped command.
        /// </summary>
        /// <param name="command">the next command in the call chain to be validated against.</param>
        /// <param name="args">the parameters (arguments) to validate.</param>
        /// <returns>A list of errors or an empty list if validation was successful.</returns>
        List<ValidationResult> Validate(ICommand command, Parameters args);
    }
}