using jmespath.lexer.StateMachines;

namespace jmespath.lexer;

public sealed class Scanner
{
    private static readonly Token T_EOF = Token.Create(TokenType.EOF, String.Empty);

    private readonly string input_;

    private int position_ = 0;
    private int line_ = 0;
    private int column_ = 0;

    private static readonly StateMachine?[] machines_ = new StateMachine?[(int)TokenType.T_LISTWILDCARD]
    {
        null, null,                 null, null, null, null, null, null,
        null, null,                 null, null, null, null, null, null,
        null, null,                 null, null, null, null, null, null,
        null, null, new UnquotedString(), null, null, null, null, null,
        null,
    };

    public Scanner(string text)
    {
        input_ = text;
    }

    public Token GetNextToken()
    {
        if (position_ >= input_.Length)
            return T_EOF;

        if (Whitespace.IsWhitespace(input_[position_]))
            Trim();

        var c = input_[position_];

        if (Char.IsAscii(c) && Char.IsLetter(c))
            return GetNextUnquotedString();

        return T_EOF;
    }

    private void Trim()
    {
        int i = 0;

        var span = input_.AsSpan(position_);
        for (var length = span.Length; i < length; ++i)
        {
            var character = span[i];

            if (!IsWhitespace(character))
                break;

            if (character == '\n')
            {
                column_ = 0;
                line_++;
            }
            else
            {
                column_++;
            }
        }

        position_ += i - position_;
    }

    private Token GetNextUnquotedString()
        => GetNextToken(TokenType.T_USTRING);

    private Token GetNextToken(TokenType tokenType, OnMatchedDelegate? onMatched = null)
    {
        System.Diagnostics.Debug.Assert(machines_[(int)tokenType] != null);
        StateMachine machine = machines_[(int)tokenType]!;

        onMatched ??= OnMatched;

        var span = input_.AsSpan(position_);
        var match = machine.Match(span);
        if (match.Success)
            return onMatched(tokenType, match.MatchedText);

        return T_EOF;
    }

    private delegate Token OnMatchedDelegate(TokenType tokenType, string text);
    private Token OnMatched(TokenType tokenType, string text)
    {
        var token = MakeToken(tokenType, text, line_, column_);

        position_ += text.Length;
        column_ += text.Length;

        return token;
    }

    #region Utils

    private static Token MakeToken(TokenType type, string text, int bLine, int bColumn, int? eLine = null, int? eColumn = null)
    {
        var token = Token.Create(type, text);
        token.Location = new LexLocation(
            bLine,
            bColumn,
            eLine ?? bLine,
            eColumn ?? bColumn + text.Length
            );

        return token;
    }

    private static bool IsWhitespace(char input)
    { 
        var ascii = 32 + (input - ' ');
        return (
              (ascii == 32) /*  ' ' */ ||
              (ascii ==  8) /* '\b' */ ||
              (ascii == 12) /* '\f' */ ||
              (ascii == 13) /* '\r' */ ||
              (ascii == 10) /* '\n' */ ||
              (ascii ==  9) /* '\t' */
            );
    }

    #endregion
}
