using System;

using PipServices4.Expressions.Variants;

namespace PipServices4.Expressions.Calculator.Variables
{
    /// <summary>
    /// Implements a variable holder object.
    /// </summary>
    public class Variable : IVariable
    {
        private string _name;
        Variant _value;

        /// <summary>
        /// Constructs this variable with name and value.
        /// </summary>
        /// <param name="name">The name of this variable.</param>
        /// <param name="value">The variable value.</param>
        public Variable(string name, Variant value = null)
        {
            if (name == null)
            {
                throw new ArgumentException("Name parameter cannot be null");
            }
            _name = name;
            _value = value ?? new Variant();
        }

        /// <summary>
        /// The variable name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The variable value.
        /// </summary>
        public Variant Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
