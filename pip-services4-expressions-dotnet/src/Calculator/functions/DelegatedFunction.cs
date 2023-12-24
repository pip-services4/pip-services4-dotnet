using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using PipServices4.Expressions.Variants;

namespace PipServices4.Expressions.Calculator.Functions
{
    /// <summary>
    /// Defines a delegate to implement a function
    /// </summary>
    /// <param name="parameters">A list with function parameters</param>
    /// <param name="variantOperations">A manager for variant operations.</param>
    /// <returns>A calculated function value.</returns>
    public delegate Task<Variant> FunctionCalculator(
        IList<Variant> parameters, IVariantOperations variantOperations);

    /// <summary>
    /// Defines an interface for expression function.
    /// </summary>
    public class DelegatedFunction : IFunction
    {
        private string _name;
        private FunctionCalculator _calculator;

        /// <summary>
        /// Constructs this function class with specified parameters.
        /// </summary>
        /// <param name="name">The name of this function.</param>
        /// <param name="calculator">The function calculator delegate.</param>
        public DelegatedFunction(string name, FunctionCalculator calculator)
        {
            if (name == null)
            {
                throw new ArgumentException("Name parameter cannot be null");
            }
            if (calculator == null)
            {
                throw new ArgumentException("Calculator parameter cannot be null");
            }
            _name = name;
            _calculator = calculator;
        }

        /// <summary>
        /// The function name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The function calculation method.
        /// </summary>
        /// <param name="parameters">A list with function parameters</param>
        /// <param name="variantOperations">Variants operations manager.</param>
        /// <returns>A calculated function result.</returns>
        public async Task<Variant> CalculateAsync(
            IList<Variant> parameters, IVariantOperations variantOperations)
        {
            return await _calculator(parameters, variantOperations);
        }
    }
}
