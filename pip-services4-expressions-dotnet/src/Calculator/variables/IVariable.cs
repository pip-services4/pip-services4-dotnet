using System;

using PipServices4.Expressions.Variants;

namespace PipServices4.Expressions.Calculator.Variables
{
    /// <summary>
    /// Defines a variable interface.
    /// </summary>
    public interface IVariable
    {
        /// <summary>
        /// The variable name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The variable value.
        /// </summary>
        Variant Value { get; set; }
    }
}
