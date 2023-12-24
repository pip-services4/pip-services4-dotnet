using System;
using System.Collections.Generic;

namespace PipServices4.Expressions.Calculator.Functions
{
    /// <summary>
    /// Implements a functions list.
    /// </summary>
    public class FunctionCollection: IFunctionCollection
    {
        private IList<IFunction> _functions = new List<IFunction>();

        /// <summary>
        /// Adds a new function to the collection.
        /// </summary>
        /// <param name="function">a function to be added.</param>
        public virtual void Add(IFunction function)
        {
            if (function == null)
            {
                throw new ArgumentException("function");
            }
            this._functions.Add(function);
        }

        /// <summary>
        /// A number of functions stored in the collection.
        /// </summary>
        public virtual int Length
        {
            get { return _functions.Count; }
        }

        /// <summary>
        /// Get a function by its index.
        /// </summary>
        /// <param name="index">a function index.</param>
        /// <returns>a retrieved function.</returns>
        public virtual IFunction Get(int index)
        {
            return _functions[index];
        }

        /// <summary>
        /// Get all functions stores in the collection
        /// </summary>
        /// <returns>a list with functions.</returns>
        public virtual IList<IFunction> GetAll()
        {
            return new List<IFunction>(_functions);
        }

        /// <summary>
        /// Finds function index in the list by it's name. 
        /// </summary>
        /// <param name="name">The function name to be found.</param>
        /// <returns>Function index in the list or <code>-1</code> if function was not found.</returns>
        public virtual int FindIndexByName(string name)
        {
            for (int i = 0; i < _functions.Count; i++)
            {
                if (_functions[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Finds function in the list by it's name.
        /// </summary>
        /// <param name="name">The function name to be found.</param>
        /// <returns>Function or <code>null</code> if function was not found.</returns>
        public virtual IFunction FindByName(string name)
        {
            int index = FindIndexByName(name);
            return index >= 0 ? _functions[index] : null;
        }

        /// <summary>
        /// Removes a function by its index.
        /// </summary>
        /// <param name="index">a index of the function to be removed.</param>
        public virtual void Remove(int index)
        {
            _functions.RemoveAt(index);
        }

        /// <summary>
        /// Removes function by it's name.
        /// </summary>
        /// <param name="name">The function name to be removed.</param>
        public virtual void RemoveByName(string name)
        {
            int index = FindIndexByName(name);
            if (index >= 0)
            {
                _functions.RemoveAt(index);
            }
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public virtual void Clear()
        {
            _functions.Clear();
        }

    }
}
