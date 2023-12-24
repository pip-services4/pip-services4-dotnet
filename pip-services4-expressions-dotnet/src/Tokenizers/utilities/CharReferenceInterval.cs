using System;

namespace PipServices4.Expressions.Tokenizers.Utilities
{
    /// <summary>
    /// Represents a character interval that keeps a reference.
    /// This class is internal and used by CharacterReferenceMap.
    /// </summary>
    sealed class CharReferenceInterval<T>
    {
        private char _start;
        private char _end;
        private T _reference;

        public CharReferenceInterval(char start, char end, T reference)
        {
            if (start > end)
            {
                throw new ArgumentException("Start must be less or equal End");
            }
            _start = start;
            _end = end;
            _reference = reference;
        }

        public char Start
        {
            get { return _start; }
        }

        public char End
        {
            get { return _end; }
        }

        public T Reference
        {
            get { return _reference; }
        }

        public bool InRange(char symbol)
        {
            return symbol >= _start && symbol <= _end;
        }
    }
}
