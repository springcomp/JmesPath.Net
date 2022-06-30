using jmespath.lexer;

public static partial class JMESPath
{
    internal static class Error
    {
        public static SyntaxErrorException Syntax(Token t)
            => Syntax(t.Type, t.Location);
        public static SyntaxErrorException Syntax(TokenType kind, LexLocation? location)
            => Syntax($"Unexpected token '{kind}'.", kind, location);
        public static SyntaxErrorException Syntax(string message, TokenType kind, LexLocation? location)
            => new($"Error({location?.StartLine ?? 0}, {location?.StartColumn ?? 0}): syntax. {message}.");

        public static SyntaxErrorException Syntax(string terminal, LexLocation? location)
            => Syntax("", terminal, location);
        public static SyntaxErrorException Syntax(string message, string terminal, LexLocation? location)
            => new($"Error({location?.StartLine ?? 0}, {location?.StartColumn ?? 0}): syntax. Unexpected terminal '{terminal}'. {message}");

        public static SyntaxErrorException Missing(TokenType kind, LexLocation? location)
            => Syntax($"Missing required token '{kind}'.", kind, location);
    }

    #region Implementation

    #endregion
}

