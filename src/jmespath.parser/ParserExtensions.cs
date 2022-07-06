using jmespath.lexer;

public static class ParserExtensions
{
    public static LexLocation? GetLocation<TState>(this Gratt.Parser<TState, TokenType, Token, int, bool> parser)
    {
        var (_, token) = parser.Peek();
        return token.Location;
    }
}

