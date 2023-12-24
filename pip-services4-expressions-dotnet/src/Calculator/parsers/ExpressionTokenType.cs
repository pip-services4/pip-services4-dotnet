namespace PipServices4.Expressions.Calculator.Parsers
{
    /// <summary>
    /// Define types of expression tokens.
    /// </summary>
    public enum ExpressionTokenType
    {
        Unknown,
        LeftBrace,
        RightBrace,
        LeftSquareBrace,
        RightSquareBrace,
        Plus,
        Minus,
        Star,
        Slash,
        Procent,
        Power,
        Equal,
        NotEqual,
        More,
        Less,
        EqualMore,
        EqualLess,
        ShiftLeft,
        ShiftRight,
        And,
        Or,
        Xor,
        Is,
        In,
        NotIn,
        Element,
        Null,
        Not,
        Like,
        NotLike,
        IsNull,
        IsNotNull,
        Comma,
        Unary,
        Function,
        Variable,
        Constant
    };
}
