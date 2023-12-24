using System;
using System.Collections.Generic;

namespace PipServices4.Expressions.Tokenizers.Utilities
{
    /// <summary>
    /// This class keeps references associated with specific characters
    /// </summary>
    public sealed class CharReferenceMap<T>
    {
        private T[] _initialInterval = new T['\x0100'];
        private IList<CharReferenceInterval<T>> _otherIntervals = new List<CharReferenceInterval<T>>();

        public CharReferenceMap()
        {
        }

        public void AddDefaultInterval(T reference)
        {
            AddInterval('\x0000', '\xfffe', reference);
        }

        public void AddInterval(char start, char end, T reference)
        {
            if (start > end)
            {
                throw new ArgumentException("Start must be less or equal End");
            }
            end = end == '\xffff' ? '\xfffe' : end;

            for (char index = start; index < '\x0100' && index <= end; index++)
            {
                _initialInterval[index] = reference;
            }
            if (end >= '\x0100')
            {
                start = start < '\x0100' ? '\x0100' : start;
                _otherIntervals.Insert(0,
                    new CharReferenceInterval<T>(start, end, reference));
            }
        }

        public void Clear()
        {
            for (char index = '\x0000'; index < '\x0100'; index++)
            {
                _initialInterval[index] = default(T);
            }
            _otherIntervals.Clear();
        }

        public T Lookup(char symbol)
        {
            if (symbol < '\x0100')
            {
                return _initialInterval[symbol];
            }
            else
            {
                foreach (CharReferenceInterval<T> interval in _otherIntervals)
                {
                    if (interval.InRange(symbol))
                    {
                        return interval.Reference;
                    }
                }
                return default(T);
            }
        }
    }
}
