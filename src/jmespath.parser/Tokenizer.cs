using jmespath.lexer;
public static class Tokenizer
{
    /// <summary>
    /// Splits the specified expression into a collection of token type and token pairs.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static IEnumerable<(TokenType TokenType, Token Token)> Tokenize(string expression)
    {
        Token? eof = null;

        var lexer = new Scanner(expression);
        while (true)
        {
            var token = lexer.GetNextToken();
            if (token.Type == TokenType.EOF || token.Type == TokenType.E_UNRECOGNIZED)
            {
                eof = token;
                break;
            }

            yield return (token.Type, token);
        }

        // Once we've reached the end of the string, just return EOF tokens. We'll
        // just keeping returning them as many times as we're asked so that the
        // parser's lookahead doesn't have to worry about running out of tokens.

        System.Diagnostics.Debug.Assert(eof != null);

        yield return (eof.Type, eof);
    }
}

