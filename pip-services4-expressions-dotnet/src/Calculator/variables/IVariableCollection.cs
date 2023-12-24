using System;
using System.Collections.Generic;

namespace PipServices4.Expressions.Calculator.Variables
{
    /// <summary>
    /// Defines a variables list.
    /// </summary>
    public interface IVariableCollection
    {
        /// <summary>
        /// Adds a new variable to the collection.
        /// </summary>
        /// <param name="variable">a variable to be added.</param>
        void Add(IVariable variable);

        /// <summary>
        /// A number of variables stored in the collection.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Get a variable by its index.
        /// </summary>
        /// <param name="index">a variable index.</param>
        /// <returns>a retrieved variable.</returns>
        IVariable Get(int index);

        /// <summary>
        /// Get all variables stores in the collection
        /// </summary>
        /// <returns>a list with variables.</returns>
        IList<IVariable> GetAll();

        /// <summary>
        /// Finds variable index in the list by it's name. 
        /// </summary>
        /// <param name="name">The variable name to be found.</param>
        /// <returns>Variable index in the list or <code>-1</code> if variable was not found.</returns>
        int FindIndexByName(string name);

        /// <summary>
        /// Finds variable in the list by it's name.
        /// </summary>
        /// <param name="name">The variable name to be found.</param>
        /// <returns>Variable or <code>null</code> if function was not found.</returns>
        IVariable FindByName(string name);

        /// <summary>
        /// Finds variable in the list or create a new one if variable was not found.
        /// </summary>
        /// <param name="name">The variable name to be found.</param>
        /// <returns>Found or created variable.</returns>
        IVariable Locate(string name);

        /// <summary>
        /// Removes a variable by its index.
        /// </summary>
        /// <param name="index">a index of the variable to be removed.</param>
        void Remove(int index);

        /// <summary>
        /// Removes variable by it's name.
        /// </summary>
        /// <param name="name">The variable name to be removed.</param>
        void RemoveByName(string name);

        /// <summary>
        /// Clears the collection.
        /// </summary>
        void Clear();

        /// <summary>
        /// Clears all stored variables (assigns null values).
        /// </summary>
        void ClearValues();
    }
}
