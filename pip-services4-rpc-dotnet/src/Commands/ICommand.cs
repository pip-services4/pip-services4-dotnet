using PipServices4.Components.Exec;
using PipServices4.Data.Validate;
using System.Collections.Generic;

namespace PipServices4.Rpc.Commands
{
    /// <summary>
    /// An interface for Commands, which are part of the Command design pattern. 
    /// Each command wraps a method or function and allows to call them in uniform and safe manner.
    /// </summary>
    /// See <see cref="Command"/>, <see cref="IExecutable"/>, <see cref="ICommandInterceptor"/>, <see cref="InterceptedCommand"/>
    public interface ICommand : IExecutable
    {
        /// <summary>
        /// Gets the command name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the command schema.
        /// </summary>
        Schema Schema { get; }

        /// <summary>
        /// Validates command arguments before execution using defined schema.
        /// </summary>
        /// <param name="args">the parameters (arguments) to validate.</param>
        /// <returns>List of errors or empty list if validation was successful.</returns>
        /// See <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/class_pip_services3_1_1_commons_1_1_run_1_1_parameters.html"/>Parameters</a>, 
        /// <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/class_pip_services3_1_1_commons_1_1_validate_1_1_validation_result.html"/>ValidationResult</a>
        IList<ValidationResult> Validate(Parameters args);
    }
}