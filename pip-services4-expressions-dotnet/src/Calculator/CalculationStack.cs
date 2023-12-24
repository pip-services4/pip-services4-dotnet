using System;
using System.Collections.Generic;

using PipServices4.Expressions.Variants;

namespace PipServices4.Expressions.Calculator
{
    /// <summary>
    /// Implements a stack of Variant values.
    /// </summary>
    public class CalculationStack
    {
        private IList<Variant> _values = new List<Variant>();

        public int Length
        {
            get { return _values.Count; }
        }

        public void Push(Variant value)
        {
            _values.Add(value);
        }

        public Variant Pop()
        {
            if (_values.Count == 0)
            {
                throw new InvalidOperationException("Stack is empty.");
            }
            Variant result = _values[_values.Count - 1];
            _values.RemoveAt(_values.Count - 1);
            return result;
        }

        public Variant PeekAt(int index)
        {
            return _values[index];
        }

        public Variant Peek()
        {
            if (_values.Count == 0)
            {
                throw new InvalidOperationException("Stack is empty.");
            }
            return _values[_values.Count - 1];
        }
    }
}
