using jmespath.lexer;

public static class ParserExtensions
{
    public static LexLocation? GetLocation<TState>(this Gratt.Parser<TState, TokenType, Token, int, bool> parser)
    {
        var (_, token) = parser.Peek();
        return token.Location;
    }

    public static Func<TokenType, (TokenType, Token), Exception> Missing<TState>(this Gratt.Parser<TState, TokenType, Token, int, bool> parser, TokenType kind)
    {
        return delegate { throw JMESPath.Error.Missing(kind, parser.GetLocation()); };
    }
}

