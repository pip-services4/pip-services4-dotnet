using System;
using System.Collections.Generic;

namespace PipServices4.Expressions.Calculator.Functions
{
    /// <summary>
    /// Defines a functions list.
    /// </summary>
    public interface IFunctionCollection
    {
        /// <summary>
        /// Adds a new function to the collection.
        /// </summary>
        /// <param name="function">a function to be added.</param>
        void Add(IFunction function);

        /// <summary>
        /// A number of functions stored in the collection.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Get a function by its index.
        /// </summary>
        /// <param name="index">a function index.</param>
        /// <returns>a retrieved function.</returns>
        IFunction Get(int index);

        /// <summary>
        /// Get all functions stores in the collection
        /// </summary>
        /// <returns>a list with functions.</returns>
        IList<IFunction> GetAll();

        /// <summary>
        /// Finds function index in the list by it's name. 
        /// </summary>
        /// <param name="name">The function name to be found.</param>
        /// <returns>Function index in the list or <code>-1</code> if function was not found.</returns>
        int FindIndexByName(string name);

        /// <summary>
        /// Finds function in the list by it's name.
        /// </summary>
        /// <param name="name">The function name to be found.</param>
        /// <returns>Function or <code>null</code> if function was not found.</returns>
        IFunction FindByName(string name);

        /// <summary>
        /// Removes a function by its index.
        /// </summary>
        /// <param name="index">a index of the function to be removed.</param>
        void Remove(int index);

        /// <summary>
        /// Removes function by it's name.
        /// </summary>
        /// <param name="name">The function name to be removed.</param>
        void RemoveByName(string name);

        /// <summary>
        /// Clears the collection.
        /// </summary>
        void Clear();

    }
}
