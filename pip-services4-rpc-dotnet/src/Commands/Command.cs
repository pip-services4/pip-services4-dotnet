using PipServices4.Commons.Errors;
using PipServices4.Components.Context;
using PipServices4.Components.Exec;
using PipServices4.Data.Validate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipServices4.Rpc.Commands
{
    public delegate Task<object> ExecutableDelegate(IContext context, Parameters args);

    /// <summary>
    /// Concrete implementation of ICommand interface. Command allows to call a method
    /// or function using Command pattern.
    /// </summary>
    /// <example>
    /// <code>
    /// var command = new Command("add", null, async(args) => {
    /// var param1 = args.GetAsFloat("param1");
    /// var param2 = args.GetAsFloat("param2");
    /// return param1 + param2; });
    /// var result = command.ExecuteAsync("123", Parameters.fromTuples(
    /// "param1", 2,
    /// "param2", 2 ));
    /// Console.WriteLine(result.ToString()); 
    /// // Console output: 4
    /// </code>
    /// </example>
    /// See <see cref="ICommand"/>, <see cref="CommandSet"/>
    public class Command : ICommand
    {
        private readonly ExecutableDelegate _function;

        /// <summary>
        /// Creates a new command object and assigns it's parameters.
        /// </summary>
        /// <param name="name">Command name.</param>
        /// <param name="schema">Command schema.</param>
        /// <param name="function"><a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/interface_pip_services3_1_1_commons_1_1_run_1_1_i_executable.html">Executable</a> function.</param>
        public Command(string name, Schema schema, ExecutableDelegate function)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            Schema = schema;
            _function = function;
        }

        /// <summary>
        /// Gets the command name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the command schema.
        /// </summary>
        public Schema Schema { get; }

        /// <summary>
        /// Executes the command. Before execution is validates Parameters args using
        /// the defined schema.The command execution intercepts exceptions raised
        /// by the called function and calls them as an error.
        /// </summary>
        /// <param name="context">Unique correlation/transaction id.</param>
        /// <param name="args">Command arguments.</param>
        /// <returns>Execution result.</returns>
        /// See <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/class_pip_services3_1_1_commons_1_1_run_1_1_parameters.html"/>Parameters</a>
        public async Task<object> ExecuteAsync(IContext context, Parameters args)
        {
            if (Schema != null)
            {
                Schema.ValidateAndThrowException(context, args);
            }

            try
            {
                return await _function.Invoke(context, args);
            }
            catch (Exception ex)
            {
                throw new InvocationException(
                    context != null ? ContextResolver.GetTraceId(context) : null,
                    "EXEC_FAILED",
                    "Execution " + Name + " failed: " + ex
                )
                .WithDetails("command", Name)
                .Wrap(ex);
            }
        }

        /// <summary>
        /// Validates the command Parameters args before execution using the defined schema.
        /// </summary>
        /// <param name="args">Command arguments.</param>
        /// <returns>a list of ValidationResults or an empty array (if no schema is set).</returns>
        /// See <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/class_pip_services3_1_1_commons_1_1_run_1_1_parameters.html"/>Parameters</a>, 
        /// <a href="https://pip-services4-dotnet.github.io/pip-services4-commons-dotnet/class_pip_services3_1_1_commons_1_1_validate_1_1_validation_result.html"/>ValidationResult</a>
        public IList<ValidationResult> Validate(Parameters args)
        {
            if (Schema != null)
            {
                return Schema.Validate(args);
            }

            return new List<ValidationResult>();
        }
    }
}