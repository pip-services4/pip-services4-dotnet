using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using PipServices4.Expressions.Variants;

namespace PipServices4.Expressions.Calculator.Functions
{
    /// <summary>
    /// Defines an interface for expression function.
    /// </summary>
    public interface IFunction
    {
        /// <summary>
        /// The function name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The function calculation method.
        /// </summary>
        /// <param name="parameters">A list with function parameters</param>
        /// <param name="variantOperations">Variants operations manager.</param>
        Task<Variant> CalculateAsync(IList<Variant> parameters, IVariantOperations variantOperations);
    }
}
