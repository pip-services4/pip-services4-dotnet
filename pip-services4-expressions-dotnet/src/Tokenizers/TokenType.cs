using System;

namespace PipServices4.Expressions.Tokenizers
{
    /// <summary>
    /// Types (categories) of tokens such as "number", "symbol" or "word".
    /// </summary>
    public enum TokenType
    {
        Unknown,
        Eof,
        Eol,
        Float,
        Integer,
        HexDecimal,
        Number,
        Symbol,
        Quoted,
        Word,
        Keyword,
        Whitespace,
        Comment,
        Special
    };
}

