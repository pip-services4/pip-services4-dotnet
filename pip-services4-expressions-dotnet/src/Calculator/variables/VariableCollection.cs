using System;
using System.Collections.Generic;

using PipServices4.Expressions.Variants;

namespace PipServices4.Expressions.Calculator.Variables
{
    /// <summary>
    /// Implements a variables list.
    /// </summary>
    public class VariableCollection: IVariableCollection
    {
        private IList<IVariable> _variables = new List<IVariable>();

        /// <summary>
        /// Adds a new variable to the collection.
        /// </summary>
        /// <param name="variable">a variable to be added.</param>
        public virtual void Add(IVariable variable)
        {
            if (variable == null)
            {
                throw new ArgumentException("variable");
            }
            this._variables.Add(variable);
        }

        /// <summary>
        /// A number of variables stored in the collection.
        /// </summary>
        public virtual int Length
        {
            get { return _variables.Count; }
        }

        /// <summary>
        /// Get a variable by its index.
        /// </summary>
        /// <param name="index">a variable index.</param>
        /// <returns>a retrieved variable.</returns>
        public virtual IVariable Get(int index)
        {
            return _variables[index];
        }

        /// <summary>
        /// Get all variables stores in the collection
        /// </summary>
        /// <returns>a list with variables.</returns>
        public virtual IList<IVariable> GetAll()
        {
            return new List<IVariable>(_variables);
        }

        /// <summary>
        /// Finds variable index in the list by it's name. 
        /// </summary>
        /// <param name="name">The variable name to be found.</param>
        /// <returns>Variable index in the list or <code>-1</code> if variable was not found.</returns>
        public virtual int FindIndexByName(string name)
        {
            for (int i = 0; i < this._variables.Count; i++)
            {
                if (_variables[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Finds variable in the list by it's name.
        /// </summary>
        /// <param name="name">The variable name to be found.</param>
        /// <returns>Variable or <code>null</code> if function was not found.</returns>
        public virtual IVariable FindByName(string name)
        {
            int index = FindIndexByName(name);
            return index >= 0 ? _variables[index] : null;
        }

        /// <summary>
        /// Finds variable in the list or create a new one if variable was not found.
        /// </summary>
        /// <param name="name">The variable name to be found.</param>
        /// <returns>Found or created variable.</returns>
        public virtual IVariable Locate(string name)
        {
            IVariable var = FindByName(name);
            if (var == null)
            {
                var = new Variable(name);
                this.Add(var);
            }
            return var;
        }

        /// <summary>
        /// Removes a variable by its index.
        /// </summary>
        /// <param name="index">a index of the variable to be removed.</param>
        public virtual void Remove(int index)
        {
            _variables.RemoveAt(index);
        }

        /// <summary>
        /// Removes variable by it's name.
        /// </summary>
        /// <param name="name">The variable name to be removed.</param>
        public virtual void RemoveByName(string name)
        {
            int index = FindIndexByName(name);
            if (index >= 0)
            {
                _variables.RemoveAt(index);
            }
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public virtual void Clear()
        {
            _variables.Clear();
        }

        /// <summary>
        /// Clears all stored variables (assigns null values).
        /// </summary>
        public virtual void ClearValues()
        {
            foreach (IVariable var in _variables)
            {
                var.Value = new Variant();
            }
        }
    }
}
