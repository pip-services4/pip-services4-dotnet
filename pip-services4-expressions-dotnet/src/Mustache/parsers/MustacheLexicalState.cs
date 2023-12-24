using System;
using System.Collections.Generic;
using System.Text;

namespace PipServices4.Expressions.Mustache.Parsers
{
    public enum MustacheLexicalState
    {
        Value = 0,
        Operator1,
        Operator2,
        Variable,
        Comment,
        Closure
    }
}
